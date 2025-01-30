namespace PeopleAPI.Models
{
    public class FriendRequest
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; } // Id użytkownika wysyłającego zaproszenie
        public Guid ReceiverId { get; set; } // Id użytkownika odbierającego zaproszenie
        public string Status { get; set; } // Pending, Accepted, Rejected
        public DateTime CreatedAt { get; set; }
    }
}
