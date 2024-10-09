using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateListingApp.Models
{
    public class Properties
    {
        [Key]
        public int PropertyID { get; set; }

        [Required(ErrorMessage = "Please Enter Property Name..")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Please Select The Property Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please Enter Address..")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter Price..")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please Enter Description..")]
        public string Description { get; set; }

        //public string Images { get; set; }

        [ValidateNever]
        [NotMapped]
        public bool IsInWishlist { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public int AgentId { get; set; }

        [ForeignKey("AgentId")]
        [ValidateNever]
        public virtual User Agent { get; set; }

    }
}
