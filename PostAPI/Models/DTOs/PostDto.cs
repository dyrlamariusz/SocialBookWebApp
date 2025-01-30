namespace PostAPI.Models.DTOs
{
    public class PostDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
