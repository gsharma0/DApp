using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }

          public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder){
              base.OnModelCreating(builder);


              builder.Entity<UserLike>().HasKey(k=> new {k.SourceUserId, k.LikedUserId});
              
              builder.Entity<UserLike>()
              .HasOne(k=>k.SourceUser)
              .WithMany(l=> l.LikedUsers)
              .HasForeignKey(k=> k.SourceUserId)
              .OnDelete(DeleteBehavior.Cascade);

               builder.Entity<UserLike>()
              .HasOne(k=>k.LikedUser)
              .WithMany(l=> l.LikedByUser)
              .HasForeignKey(k=> k.LikedUserId)
              .OnDelete(DeleteBehavior.Cascade);

              builder.Entity<Message>()
              .HasOne(u=>u.Recipient)
              .WithMany(m=>m.MessageRecieved)
              .OnDelete(DeleteBehavior.Restrict);

               builder.Entity<Message>()
              .HasOne(u=>u.Sender)
              .WithMany(m=>m.MessageSent)
              .OnDelete(DeleteBehavior.Restrict);
          }
    }
}