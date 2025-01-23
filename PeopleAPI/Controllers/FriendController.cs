using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.Models;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendFriendRequest([FromBody] FriendRequest request)
        {
            request.Id = Guid.NewGuid();
            request.Status = "Pending";
            request.CreatedAt = DateTime.UtcNow;

            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie wysłane.");
        }

        [HttpPatch("{id}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(Guid id)
        {
            var request = await _context.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == id);
            if (request == null)
            {
                return NotFound("Zaproszenie nie istnieje.");
            }

            request.Status = "Accepted";
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie zaakceptowane.");
        }

        [HttpPatch("{id}/reject")]
        [Authorize]
        public async Task<IActionResult> RejectFriendRequest(Guid id)
        {
            var request = await _context.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == id);
            if (request == null)
            {
                return NotFound("Zaproszenie nie istnieje.");
            }

            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            return Ok("Zaproszenie odrzucone.");
        }
    }
}
