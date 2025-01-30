using PeopleAPI.Models.DTOs;

namespace PeopleAPI.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<FriendDto> Friends { get; set; } = new List<FriendDto>();
    }
}
