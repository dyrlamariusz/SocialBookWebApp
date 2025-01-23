using System.Net.Http.Json;

public class PeopleService
{
    private readonly HttpClient _httpClient;

    public PeopleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<dynamic> GetProfile(string userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<dynamic>($"http://localhost:5002/api/profile/{userId}");
        }
        catch
        {
            return null;
        }
    }
}
