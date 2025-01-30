using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.Models;
using System.Security.Claims;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/friend-requests")]
    public class FriendRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Wysyłanie zaproszenia do znajomych
        [HttpPost("{receiverId}")]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest(string receiverIdString)
        {
            var senderIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(senderIdString, out Guid senderId);
            Guid.TryParse(receiverIdString, out Guid receiverId);
            if (senderId == receiverId) return BadRequest("Nie możesz wysłać zaproszenia do siebie.");

            // Sprawdzenie, czy zaproszenie już istnieje
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if (existingRequest != null)
            {
                return BadRequest("Zaproszenie już zostało wysłane.");
            }

            // Sprawdzenie, czy użytkownicy nie są już znajomymi
            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == senderId && f.FriendId == receiverId) || (f.UserId == receiverId && f.FriendId == senderId));

            if (existingFriendship != null)
            {
                return BadRequest("Już jesteście znajomymi.");
            }

            var friendRequest = new FriendRequest
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie wysłane.");
        }

        // ✅ Akceptowanie zaproszenia do znajomych
        [HttpPost("{requestId}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(string requestIdString)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);
            Guid.TryParse(requestIdString, out Guid requestId);
            var request = await _context.FriendRequests.FindAsync(requestId);

            if (request == null || request.ReceiverId != userId)
            {
                return NotFound("Zaproszenie nie istnieje lub nie jesteś jego odbiorcą.");
            }

            request.Status = "Accepted";
            _context.FriendRequests.Update(request);

            // Tworzenie znajomości po akceptacji zaproszenia
            var friendship = new Friendship
            {
                Id = Guid.NewGuid(),
                UserId = request.SenderId,
                FriendId = request.ReceiverId
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie zaakceptowane, dodano do znajomych.");
        }

        // ✅ Odrzucanie zaproszenia do znajomych
        [HttpPost("{requestId}/reject")]
        [Authorize]
        public async Task<IActionResult> RejectFriendRequest(string requestIdString)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);
            Guid.TryParse(requestIdString, out Guid requestId);
            var request = await _context.FriendRequests.FindAsync(requestId);

            if (request == null || request.ReceiverId != userId)
            {
                return NotFound("Zaproszenie nie istnieje lub nie jesteś jego odbiorcą.");
            }

            request.Status = "Rejected";
            _context.FriendRequests.Update(request);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie odrzucone.");
        }

        // ✅ Pobieranie zaproszeń użytkownika
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFriendRequests()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);
            var requests = await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && fr.Status == "Pending")
                .ToListAsync();

            return Ok(requests);
        }
    }
}
