namespace PeopleAPI.Models
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } // Powiązanie z IdentityAPI
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
