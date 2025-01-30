using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleAPI.Models
{
    public class Friendship
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Profile User { get; set; }

        [Required]
        public Guid FriendId { get; set; }
        [ForeignKey(nameof(FriendId))]
        public Profile Friend { get; set; }
    }
}
