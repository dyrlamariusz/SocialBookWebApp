using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeopleAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/people")]
    public class PeopleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PeopleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("suggested")]
        [Authorize]
        public async Task<IActionResult> GetSuggestedFriends()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Nie można znaleźć użytkownika.");

            if (!Guid.TryParse(userIdString, out Guid userId))
                return BadRequest("Nieprawidłowy identyfikator użytkownika.");

            var allUsers = await _context.Users
                .Where(u => u.Id != userId) //UPEWNIC SIE ZE W BAZIE JEST STRIGNIEM userId.ParseToString
                .ToListAsync();

            return Ok(allUsers);
        }


        // ✅ Dodawanie znajomego
        [HttpPost("{friendId}/add")]
        [Authorize]
        public async Task<IActionResult> AddFriend(string friendId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized("Nie można znaleźć użytkownika.");

            if (!Guid.TryParse(userIdString, out Guid userId))
                return BadRequest("Nieprawidłowy identyfikator użytkownika.");

            if (!Guid.TryParse(friendId, out Guid friendGuid))
                return BadRequest("Nieprawidłowy identyfikator znajomego.");

            if (userId == friendGuid)
                return BadRequest("Nie możesz dodać siebie do znajomych.");

            // Znajdź profil użytkownika
            var userProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            var friendProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == friendGuid);

            if (userProfile == null || friendProfile == null)
                return NotFound("Nie znaleziono użytkownika lub znajomego.");

            // Upewnij się, że znajomość jeszcze nie istnieje
            var existingFriendship = await _context.Friendships.FirstOrDefaultAsync(f =>
                (f.UserId == userId && f.FriendId == friendGuid) ||
                (f.UserId == friendGuid && f.FriendId == userId));

            if (existingFriendship != null)
                return BadRequest("Już jesteście znajomymi.");

            // Tworzenie nowej znajomości
            var friendship = new Friendship
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FriendId = friendGuid
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();

            return Ok("Dodano do znajomych.");
        }
    }
}
