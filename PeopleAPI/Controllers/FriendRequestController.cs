using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.DTOs;
using PeopleAPI.Models;
using System.Net.Http.Headers;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/friend-requests")]
    public class FriendRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public FriendRequestController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
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

        [HttpPost("{receiverId}")]
        public async Task<IActionResult> SendFriendRequest(
            [FromHeader(Name = "Authorization")] string authHeader,
            string receiverId)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var senderId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(senderId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            if (senderId == receiverId)
                return BadRequest("Nie możesz wysłać zaproszenia do siebie.");

            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if (existingRequest != null)
                return BadRequest("Zaproszenie już zostało wysłane.");

            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == senderId && f.FriendId == receiverId) || (f.UserId == receiverId && f.FriendId == senderId));

            if (existingFriendship != null)
                return BadRequest("Już jesteście znajomymi.");

            var friendRequest = new FriendRequest
            {
                Id = Guid.NewGuid().ToString(), // Generowanie ID jako string
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie wysłane.");
        }

        [HttpPost("{requestId}/accept")]
        public async Task<IActionResult> AcceptFriendRequest(
            [FromHeader(Name = "Authorization")] string authHeader,
            string requestId)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var request = await _context.FriendRequests.FindAsync(requestId);
            if (request == null || request.ReceiverId != userId)
                return NotFound("Zaproszenie nie istnieje lub nie jesteś jego odbiorcą.");

            request.Status = "Accepted";
            _context.FriendRequests.Update(request);

            var friendship = new Friendship
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.SenderId,
                FriendId = request.ReceiverId
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie zaakceptowane, dodano do znajomych.");
        }

        [HttpPost("{requestId}/reject")]
        public async Task<IActionResult> RejectFriendRequest(
            [FromHeader(Name = "Authorization")] string authHeader,
            string requestId)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var request = await _context.FriendRequests.FindAsync(requestId);
            if (request == null || request.ReceiverId != userId)
                return NotFound("Zaproszenie nie istnieje lub nie jesteś jego odbiorcą.");

            request.Status = "Rejected";
            _context.FriendRequests.Update(request);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie odrzucone.");
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendRequests(
            [FromHeader(Name = "Authorization")] string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var requests = await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status == "Pending")
                .ToListAsync();

            return Ok(requests);
        }
    }
}
