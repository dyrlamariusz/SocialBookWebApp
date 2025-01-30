using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostAPI.Data;
using PostAPI.Models;
using System.Security.Claims;

namespace PostAPI.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
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
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Nie można znaleźć użytkownika.");

            post.Id = Guid.NewGuid();
            post.CreatedAt = DateTime.UtcNow;
            post.UserId = userId;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(post);
        }

        [HttpPost("{postId}/comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid postId, [FromBody] Comment comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Nie można znaleźć użytkownika.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return NotFound("Post nie znaleziony.");

            comment.Id = Guid.NewGuid();
            comment.PostId = postId;
            comment.UserId = userId;
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpPost("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("Nie można znaleźć użytkownika.");

            var post = await _context.Posts.Include(p => p.Likes).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null) return NotFound("Post nie znaleziony.");

            if (post.Likes.Any(l => l.UserId == userId))
            {
                return BadRequest("Już polubiłeś ten post.");
            }

            var like = new Like
            {
                Id = Guid.NewGuid(),
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
