namespace PostAPI.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
