namespace RealEstateListingApp.Models
{
    public class PropertyViewModel
    {
        public Properties Property { get; set; }
        public bool IsInWishlist { get; set; }
        public IEnumerable<Properties> Properties { get; set; }

    }
}
