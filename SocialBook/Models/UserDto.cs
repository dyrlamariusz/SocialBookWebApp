namespace SocialBook.Models
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<FriendDto> Friends { get; set; } = new List<FriendDto>();
    }
}
