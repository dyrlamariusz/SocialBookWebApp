using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeopleAPI.Models
{
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string ProfilePictureUrl { get; set; }
        public ICollection<Friendship> SentFriendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> ReceivedFriendships { get; set; } = new List<Friendship>();
    }
}
