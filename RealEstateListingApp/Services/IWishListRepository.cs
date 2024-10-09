using RealEstateListingApp.Models;

namespace RealEstateListingApp.Services
{
    public interface IWishListRepository
    {
        void AddPropertyInWishlist(Wishlists data);
        void DeletePropertyFromWishlist(Wishlists data);
        IEnumerable<Wishlists> GetWishlists();

        Wishlists GetWishlist(int id);
    }

}
