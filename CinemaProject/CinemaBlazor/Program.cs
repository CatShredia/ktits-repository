using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using CinemaBlazor;
using CinemaBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with base address from environment or default
var apiBaseAddress = builder.Configuration["API_BASE_ADDRESS"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseAddress) };
    return httpClient;
});

// Register services
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFilmService, FilmService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IRatingService, RatingService>();

// Add Authorization
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();