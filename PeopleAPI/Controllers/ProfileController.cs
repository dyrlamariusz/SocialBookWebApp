using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.DTOs;
using PeopleAPI.Models;
using PeopleAPI.Models.DTOs;
using System.Security.Claims;

namespace PeopleAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);
            var profile = await _context.Profiles
                .Include(p => p.SentFriendships)
                    .ThenInclude(f => f.Friend)
                .Include(p => p.ReceivedFriendships)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return NotFound("Profil nie znaleziony.");

            var userDto = new UserDto
            {
                Id = profile.UserId,
                FullName = profile.FullName,
                Email = profile.Email,
                Friends = profile.SentFriendships.Select(f => new FriendDto
                {
                    Id = f.Friend.UserId,
                    FullName = f.Friend.FullName
                })
                .Concat(profile.ReceivedFriendships.Select(f => new FriendDto
                {
                    Id = f.User.UserId,
                    FullName = f.User.FullName
                }))
                .ToList()
            };

            return Ok(userDto);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] Profile updatedProfile)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdString, out Guid userId);
            if (userId == null) return Unauthorized("Nie można znaleźć użytkownika.");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null) return NotFound("Profil nie istnieje.");

            profile.FullName = updatedProfile.FullName;
            profile.Bio = updatedProfile.Bio;
            profile.Location = updatedProfile.Location;
            profile.ProfilePictureUrl = updatedProfile.ProfilePictureUrl;

            await _context.SaveChangesAsync();
            return Ok(profile);
        }
    }
}
