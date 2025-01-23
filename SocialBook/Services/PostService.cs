using System.Net.Http.Json;

public class PostService
{
    private readonly HttpClient _httpClient;

    public PostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<dynamic> GetAllPosts()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<dynamic>("http://localhost:5003/api/post");
        }
        catch
        {
            return null;
        }
    }
}
