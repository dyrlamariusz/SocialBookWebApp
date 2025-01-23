using System.Net.Http.Json;

public class IdentityService
{
    private readonly HttpClient _httpClient;

    public IdentityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> Login(string username, string password)
    {
        var noo = $"{_httpClient.BaseAddress}Auth/login";

        var response = await _httpClient.PostAsJsonAsync("Auth/login", new
        {
            username,
            password
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<dynamic>();
            return result.GetProperty("token").GetString();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {error}");
        }
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
