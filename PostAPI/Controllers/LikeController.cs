using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostAPI.Data;
using PostAPI.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LikeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("{postId}")]
    [Authorize]
    public async Task<IActionResult> LikePost(Guid postId)
    {
        var like = new Like
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            LikedAt = DateTime.UtcNow
        };

        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        return Ok("Polubiono post.");
    }
}
