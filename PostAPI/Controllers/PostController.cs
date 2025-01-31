using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostAPI.Data;
using PostAPI.DTOs;
using PostAPI.Models;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace PostAPI.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public PostController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
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
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(
            [FromHeader(Name = "Authorization")] string authHeader,
            [FromBody] Post post)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            post.Id = Guid.NewGuid().ToString();
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = userId;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(post);
        }

        [HttpPost("{postId}/comment")]
        public async Task<IActionResult> AddComment(
            [FromHeader(Name = "Authorization")] string authHeader,
            string postId,
            [FromBody] Comment comment)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Brak tokena autoryzacyjnego.");

            var token = authHeader.Substring(7);
            var userId = await GetUserIdFromIdentityAPI(token);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nie można zweryfikować użytkownika.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound("Post nie znaleziony.");

            comment.Id = Guid.NewGuid().ToString();
            comment.PostId = postId;
            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpPost("{postId}/like")]
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

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) return NotFound("Post nie znaleziony.");

            if (post.Likes.Any(l => l.UserId == userId))
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
}
