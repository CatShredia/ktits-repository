using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductionSystem.Api;
using ProductionSystem.Api.Services;
using ProductionSystem.Data;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Default")
           ?? "Host=localhost;Database=production_system;Username=postgres;Password=postgres";

if (builder.Environment.IsEnvironment("Testing"))
{
    var inMemoryDbName = builder.Configuration["InMemoryDatabaseName"] ?? "ProductionSystemTests";
    builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(inMemoryDbName));
}
else
    builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddSingleton<JwtTokenBuilder>();
builder.Services.AddScoped<ProductRequirementsService>();
builder.Services.AddScoped<ProcurementEstimationService>();
builder.Services.AddScoped<ProductionSchedulingService>();
builder.Services.AddScoped<MaterialWriteOffService>();
builder.Services.AddScoped<OrderWorkflowService>();
builder.Services.AddControllers();
builder.Services.AddProductionSystemSwagger();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()));

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is required");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = signingKey,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseProductionSystemSwagger();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        await db.Database.MigrateAsync();
        await WorkshopSeedService.EnsureSeededAsync(db, env);
    }
}

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger", permanent: false));

await app.RunAsync();

public partial class Program;
