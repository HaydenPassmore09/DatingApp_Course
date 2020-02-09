
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Models.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Setting the primarykey of the Like Entity because this entity does not have a property called id.
            // Which by convention Entity framework uses id as the primary key
            builder.Entity<Like>().HasKey( k => new {k.LikerId, k.LikeeId});

            builder.Entity<Like>().HasOne(u => u.Likee)
            .WithMany(u => u.Likers)
            .HasForeignKey(u => u.LikeeId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>().HasOne(u => u.Liker)
            .WithMany(u => u.Likees)
            .HasForeignKey(u => u.LikerId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}