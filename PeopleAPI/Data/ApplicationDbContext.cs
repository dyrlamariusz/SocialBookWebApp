using Microsoft.EntityFrameworkCore;
using PeopleAPI.Models;

namespace PeopleAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracja relacji wiele-do-wielu dla znajomości
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(p => p.SentFriendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(p => p.ReceivedFriendships)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
