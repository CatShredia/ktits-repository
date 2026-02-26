using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TestBlazorAssembly;
using TestBlazorAssembly.ApiRequest;
using Microsoft.AspNetCore.Components.Authorization;
using TestBlazorAssembly.ApiRequest.Services;
using TestBlazorAssembly.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped<ApiRequestService>();

// auth-related services -------------------------------------------------
// чтобы UserService мог получить AuthenticationStateProvider и работал
// механизм авторизации Blazor
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
// наша собственная реализация состояния авторизации
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<UserService>();

// HTTP-клиент (можно и раньше, но удобно объявить после настроек)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5039/") });

await builder.Build().RunAsync();