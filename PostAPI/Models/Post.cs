namespace PostAPI.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } // Powiązanie z IdentityAPI
        public string Content { get; set; } // Tekst postu
        public string ImageUrl { get; set; } // Opcjonalny link do obrazka
        public DateTime CreatedAt { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
