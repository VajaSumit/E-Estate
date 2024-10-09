using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateListingApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter user name..")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter password..")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select role..")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Please enter contact number..")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must contain exactly 10 digits.")]
        public string ContactInfo { get; set; }

        [ValidateNever]
        [NotMapped]
        public bool rememberMe { get; set; }

    }
}
