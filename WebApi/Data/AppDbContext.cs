using Microsoft.EntityFrameworkCore;
using WebApi.Data.Models;

namespace WebApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cat>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Cats)
                .UsingEntity<Dictionary<string, object>>(
                    "CatTag",
                    cat => cat.HasOne<Tag>().WithMany().HasForeignKey("TagId"),
                    tag => tag.HasOne<Cat>().WithMany().HasForeignKey("CatId")
                );

            modelBuilder.Entity<Cat>().HasIndex(op => op.CatId).IsUnique();
            modelBuilder.Entity<Cat>().Property(c => c.Image).IsRequired();

            modelBuilder.Entity<Tag>().HasIndex(op => op.Name).IsUnique();
            modelBuilder.Entity<Tag>().Property(t => t.Name).IsRequired();
        }
    }
}
