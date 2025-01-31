using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SocialBook.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class PostService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public PostService(HttpClient httpClient, ProtectedLocalStorage localStorage, AuthenticationStateProvider authStateProvider)
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

    public async Task<List<PostDto>> GetAllPosts()
    {
        return await _httpClient.GetFromJsonAsync<List<PostDto>>("http://localhost:5003/api/posts");
    }

    public async Task CreatePost(string content)
    {
        await SetAuthorizationHeaderAsync();
        var newPost = new { Content = content };
        await _httpClient.PostAsJsonAsync("http://localhost:5003/api/posts", newPost);
    }

    public async Task LikePost(string postId)
    {
        await SetAuthorizationHeaderAsync();
        await _httpClient.PostAsync($"http://localhost:5003/api/likes/{postId}", null);
    }

    public async Task AddComment(string postId, string content)
    {
        await SetAuthorizationHeaderAsync();
        var newComment = new { Content = content };
        await _httpClient.PostAsJsonAsync($"http://localhost:5003/api/posts/{postId}/comment", newComment);
    }
}
