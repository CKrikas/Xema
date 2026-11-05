using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using xema.Models;

namespace xema.Data
{
    public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TattooImage> TattooImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ContactRequest> ContactRequests { get; set; }
        public DbSet<Media> Media { get; set; }

        public DbSet<HeroBanner> HeroBanners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TattooImage>()
                .HasOne(t => t.Media)
                .WithMany()
                .HasForeignKey(t => t.MediaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
