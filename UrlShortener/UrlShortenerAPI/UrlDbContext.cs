using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Entities;

namespace UrlShortenerAPI
{
    public class UrlDbContext : DbContext
    {
        public UrlDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ShortUrl> shorturls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrl>(entity => {

                entity.HasKey(e => e.id);
                // make sure that the shortened codes are all unique
                entity.HasIndex(s => s.urlcode).IsUnique();

            });
        }
    }
}
