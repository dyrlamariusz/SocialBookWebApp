using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SocialBook.Models;
using System.Text.Json;

public class IdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public IdentityService(HttpClient httpClient, ProtectedLocalStorage localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        var tokenResult = await _localStorage.GetAsync<string>("jwt_token");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }
    }

    public async Task<UserDto?> GetCurrentUser()
    {
        try
        {
            await SetAuthorizationHeaderAsync();
            Console.WriteLine(_httpClient);
            var user = await _httpClient.GetFromJsonAsync<UserDto>("Auth/me");
            return user;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching user: {ex.Message}");
            return null;
        }
    }

    public async Task<string?> Login(string username, string password)
    {
        var loginRequest = new { Username = username, Password = password };
        var response = await _httpClient.PostAsJsonAsync("Auth/login", loginRequest);

        Console.WriteLine($"Response Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"🔍 Response JSON: {json}");

        var loginResult = JsonSerializer.Deserialize<JsonElement>(json);

        foreach (var property in loginResult.EnumerateObject())
        {
            Console.WriteLine($"🔑 Found key: {property.Name} => {property.Value}");
        }

        if (loginResult.TryGetProperty("Token", out var tokenElement) ||
            loginResult.TryGetProperty("token", out tokenElement) ||
            loginResult.TryGetProperty("access_token", out tokenElement) ||
            loginResult.TryGetProperty("accessToken", out tokenElement))
        {
            string? token = tokenElement.GetString();

            if (!string.IsNullOrEmpty(token))
            {
                await _localStorage.SetAsync("jwt_token", token);
                return token;
            }
        }
        return null;
    }

    public async Task<bool> Register(string username, string email, string fullName, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/register", new
        {
            username,
            email,
            fullName,
            password
        });

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Registration failed: {error}");
        }
    }
}
