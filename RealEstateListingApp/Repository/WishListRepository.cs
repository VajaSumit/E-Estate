using Microsoft.EntityFrameworkCore;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;

namespace RealEstateListingApp.Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly DbContextFile db;
        private readonly DbSet<Wishlists> wishlists;

        public WishListRepository(DbContextFile db)
        {
            this.db = db;
            wishlists = db.Set<Wishlists>();
        }

        public void AddPropertyInWishlist(Wishlists data)
        {
            db.Wishlists.Add(data);
            db.SaveChanges();

        }

        public void DeletePropertyFromWishlist(Wishlists data)
        {
            db.Wishlists.Remove(data);
            db.SaveChanges();
        }

        public Wishlists GetWishlist(int id)
        {
            return db.Wishlists.Include(u => u.User).Include(p => p.Properties).Where(x => x.WishlistId == id).FirstOrDefault();
        }

        public IEnumerable<Wishlists> GetWishlists()
        {
            return db.Wishlists.Include(u => u.User).Include(p => p.Properties).ToList();
        }
    }
}
