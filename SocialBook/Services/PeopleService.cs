﻿using Microsoft.AspNetCore.Components.Authorization;
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

    private async Task SetAuthorizationHeaderAsync()
    {
        var tokenResult = await _localStorage.GetAsync<string>("jwt_token");
        if (tokenResult.Success && !string.IsNullOrEmpty(tokenResult.Value))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.Value);
        }
    }

    public async Task<UserDto?> GetProfile(Guid userId)
    {
        await SetAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<UserDto>($"people/profile/{userId}");
    }

    public async Task<List<UserDto>> GetSuggestedFriends()
    {
        await SetAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("people/suggested");
    }

    public async Task AddFriend(string friendId)
    {
        await SetAuthorizationHeaderAsync();
        await _httpClient.PostAsync($"people/{friendId}/add", null);
    }

    public async Task<UserDto?> GetCurrentUserProfile()
    {
        await SetAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<UserDto>("profile/profile");
    }

    public async Task<List<FriendRequest>> GetPendingFriendRequests()
    {
        await SetAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<List<FriendRequest>>("friend-requests");
    }

    public async Task SendFriendRequest(Guid receiverId)
    {
        await SetAuthorizationHeaderAsync();
        await _httpClient.PostAsync($"friend-requests/{receiverId}", null);
    }

    public async Task AcceptFriendRequest(Guid requestId)
    {
        await SetAuthorizationHeaderAsync();
        await _httpClient.PostAsync($"friend-requests/{requestId}/accept", null);
    }

    public async Task RejectFriendRequest(Guid requestId)
    {
        await SetAuthorizationHeaderAsync();
        await _httpClient.PostAsync($"friend-requests/{requestId}/reject", null);
    }
}
