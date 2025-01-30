using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PeopleAPI.Models
{
    public class Profile
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }  // ID użytkownika z IdentityAPI
        public string FullName { get; set; }
        public string Email { get; set; } // ✅ Dodaj Email
        public string Bio { get; set; }
        public string Location { get; set; }
        public string ProfilePictureUrl { get; set; }

        // ✅ Relacje wiele-do-wielu (poprawiona struktura)
        public ICollection<Friendship> SentFriendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> ReceivedFriendships { get; set; } = new List<Friendship>();
    }
}
