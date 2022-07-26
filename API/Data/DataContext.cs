using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data
{
    //public class DataContext : DbContext Commented to use Identity feature and tables
    public class DataContext : IdentityDbContext<AppUser, AppRole , int, IdentityUserClaim<int>, AppUserRole,
    IdentityUserLogin<int>, IdentityRoleClaim<int>,  IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        //Will use Identity tables
        //public DbSet<AppUser> Users { get; set; }

          public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Connection> Connections { get; set; }
        protected override void OnModelCreating(ModelBuilder builder){
              base.OnModelCreating(builder);


            builder.Entity<AppUser>()
            .HasMany(u=>u.UserRoles)
            .WithOne(u=>u.User)
            .HasForeignKey(u=>u.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(u=>u.UserRoles)
            .WithOne(u=>u.Role)
            .HasForeignKey(u=>u.RoleId)
            .IsRequired();

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