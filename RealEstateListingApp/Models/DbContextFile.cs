using Microsoft.EntityFrameworkCore;

namespace RealEstateListingApp.Models
{
    public class DbContextFile : DbContext
    {
        public DbContextFile(DbContextOptions dbContextOptions) : base(dbContextOptions)
        { 
             
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Properties> Properties { get; set; }

        public DbSet<Wishlists> Wishlists { get; set; }

        public DbSet<Inquiries> Inquiries { get; set; }

    }
}
