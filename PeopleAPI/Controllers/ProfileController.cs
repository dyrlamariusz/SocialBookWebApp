using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.Models;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
            if (profile == null)
            {
                return NotFound("Profil nie znaleziony.");
            }

            return Ok(profile);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProfile([FromBody] Profile profile)
        {
            profile.Id = Guid.NewGuid();
            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return Ok(profile);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] Profile updatedProfile)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
            if (profile == null)
            {
                return NotFound("Profil nie istnieje.");
            }

            profile.FullName = updatedProfile.FullName;
            profile.Bio = updatedProfile.Bio;
            profile.Location = updatedProfile.Location;
            profile.ProfilePictureUrl = updatedProfile.ProfilePictureUrl;

            await _context.SaveChangesAsync();
            return Ok(profile);
        }
    }
}
