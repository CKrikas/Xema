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
    }
}
