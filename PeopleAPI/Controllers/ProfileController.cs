using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleAPI.Data;
using PeopleAPI.DTOs;
using PeopleAPI.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PeopleAPI.Controllers
{
    [ApiController]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ProfileController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
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

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserProfile([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var userProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (userProfile == null)
            {
                userProfile = new Profile
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    FullName = "Nowy Użytkownik",
                    Email = "",
                    Bio = "",
                    Location = "",
                    ProfilePictureUrl = ""
                };

                _context.Profiles.Add(userProfile);
                await _context.SaveChangesAsync();
            }

            return Ok(userProfile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromHeader(Name = "Authorization")] string authHeader, [FromBody] Profile updatedProfile)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
                return NotFound("Profil nie istnieje.");

            profile.FullName = updatedProfile.FullName;
            profile.Bio = updatedProfile.Bio;
            profile.Location = updatedProfile.Location;
            profile.ProfilePictureUrl = updatedProfile.ProfilePictureUrl;

            await _context.SaveChangesAsync();
            return Ok(profile);
        }
    }
}
