using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SocialBook.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class PeopleService
{
    private readonly HttpClient _httpClient;
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public PeopleService(HttpClient httpClient, ProtectedLocalStorage localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<UserDto?> GetProfile(Guid userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"http://localhost:5002/api/profile/{userId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserDto>> GetSuggestedFriends()
    {
         
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("http://localhost:5002/api/people/suggested");
    }

    public async Task AddFriend(string friendId)
    {
         
        await _httpClient.PostAsync($"http://localhost:5002/api/people/{friendId}/add", null);
    }

    public async Task<UserDto?> GetCurrentUserProfile()
    {
        var tokenResult = await _localStorage.GetAsync<string>("jwt_token");

        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }

        try
        {
            // ✅ Ensure this URL matches your API
            return await _httpClient.GetFromJsonAsync<UserDto>("api/People/profile");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching profile: {ex.Message}");
            return null;
        }
    }


    public async Task<List<FriendRequest>> GetPendingFriendRequests()
    {
         
        return await _httpClient.GetFromJsonAsync<List<FriendRequest>>("http://localhost:5002/api/friend-requests");
    }

    public async Task SendFriendRequest(Guid receiverId)
    {
         
        await _httpClient.PostAsync($"http://localhost:5002/api/friend-requests/{receiverId}", null);
    }

    public async Task AcceptFriendRequest(Guid requestId)
    {
         
        await _httpClient.PostAsync($"http://localhost:5002/api/friend-requests/{requestId}/accept", null);
    }

    public async Task RejectFriendRequest(Guid requestId)
    {
         
        await _httpClient.PostAsync($"http://localhost:5002/api/friend-requests/{requestId}/reject", null);
    }
}
