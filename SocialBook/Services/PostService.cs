using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SocialBook.Models;
using System.Net.Http.Headers;

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

    // ✅ Pobieranie postów
    public async Task<List<PostDto>> GetAllPosts()
    {
        return await _httpClient.GetFromJsonAsync<List<PostDto>>("http://localhost:5003/api/posts");
    }

    public async Task CreatePost(string content)
    {
        var newPost = new { Content = content };
        await _httpClient.PostAsJsonAsync("http://localhost:5003/api/posts", newPost);
    }

    public async Task LikePost(string postId)
    {
        await _httpClient.PostAsync($"http://localhost:5003/api/posts/{postId}/like", null);
    }

    public async Task AddComment(string postId, string content)
    {
        var newComment = new { Content = content };
        await _httpClient.PostAsJsonAsync($"http://localhost:5003/api/posts/{postId}/comment", newComment);
    }

}
