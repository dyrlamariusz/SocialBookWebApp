using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.DTOs;
using PeopleAPI.Models;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public PeopleController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
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

        [HttpGet("suggested")]
        public async Task<IActionResult> GetSuggestedFriends([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var allUsers = await _context.Users
                .Where(u => u.Id != userId)
                .ToListAsync();

            return Ok(allUsers);
        }

        [HttpPost("{friendId}/add")]
        public async Task<IActionResult> AddFriend(
            [FromHeader(Name = "Authorization")] string authHeader,
            string friendId)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            if (userId == friendId)
                return BadRequest("Nie możesz dodać siebie do znajomych.");

            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            var friendProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == friendId);

            if (userProfile == null || friendProfile == null)
                return NotFound("Nie znaleziono użytkownika lub znajomego.");

            var existingFriendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.UserId == userId && f.FriendId == friendId) ||
                (f.UserId == friendId && f.FriendId == userId));

            if (existingFriendship != null)
                return BadRequest("Już jesteście znajomymi.");

            var friendship = new Friendship
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FriendId = friendId
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Dodano do znajomych.");
        }
    }
}
