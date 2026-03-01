using Microsoft.EntityFrameworkCore;
using TestApi3K.Database;
using TestApi3K.Services.Interfaces;
using TestApi3K.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextDb>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("TestDbString")));

builder.Services.AddScoped<IUserRepository, UserLoginService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyPort", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAnyPort");

app.MapControllers();

app.Run();
