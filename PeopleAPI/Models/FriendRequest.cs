namespace PeopleAPI.Models
{
    public class FriendRequest
    {
        public string Id { get; set; }
        public string SenderId { get; set; } 
        public string ReceiverId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
