namespace PeopleAPI.Models
{
    public class FriendRequest
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; } // Id użytkownika wysyłającego zaproszenie
        public string ReceiverId { get; set; } // Id użytkownika odbierającego zaproszenie
        public string Status { get; set; } // Pending, Accepted, Rejected
        public DateTime CreatedAt { get; set; }
    }
}
