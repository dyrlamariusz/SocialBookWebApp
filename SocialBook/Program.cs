using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5001/") };
    return httpClient;
});

builder.Services.AddScoped<ProtectedLocalStorage>();

builder.Services.AddHttpClient<IdentityService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/");
});
builder.Services.AddHttpClient<PeopleService>(client =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var authProvider = serviceProvider.GetRequiredService<CustomAuthenticationStateProvider>();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authProvider.GetToken());
    client.BaseAddress = new Uri("http://localhost:5002/api/");
});
builder.Services.AddHttpClient<PostService>(client =>
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    var authProvider = serviceProvider.GetRequiredService<CustomAuthenticationStateProvider>();
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authProvider.GetToken());
    client.BaseAddress = new Uri("http://localhost:5003/api/");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
