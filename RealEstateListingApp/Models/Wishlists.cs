using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateListingApp.Models
{
    public class Wishlists
    {
        [Key]
        public int WishlistId { get; set; }

        public int UserId { get; set; }

        public int PropertyId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Properties Properties { get; set; }
    }
}
