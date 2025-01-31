namespace PostAPI.Models
{
    public class Like
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public DateTime LikedAt { get; set; }
    }
}
