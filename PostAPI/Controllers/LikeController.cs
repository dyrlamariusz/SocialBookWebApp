using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostAPI.Data;
using PostAPI.DTOs;
using PostAPI.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

[ApiController]
[Route("api/likes")]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public LikeController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    private async Task<string?> GetUserIdFromIdentityAPI(string token)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "http://identity-api:8080/api/Auth/me");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            return userDto?.Id;
        }
        catch
        {
            return null;
        }
    }

    [HttpPost("{postId}")]
    public async Task<IActionResult> LikePost(
        [FromHeader(Name = "Authorization")] string authHeader,
        string postId)
    {
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized("Brak tokena autoryzacyjnego.");

        var token = authHeader.Substring(7);
        var userId = await GetUserIdFromIdentityAPI(token);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Nie można zweryfikować użytkownika.");

        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
            return NotFound("Post nie znaleziony.");

        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike != null)
            return BadRequest("Już polubiłeś ten post.");

        var like = new Like
        {
            Id = Guid.NewGuid().ToString(),
            PostId = postId,
            UserId = userId,
            LikedAt = DateTime.UtcNow
        };

        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        return Ok("Polubiono post.");
    }
}
