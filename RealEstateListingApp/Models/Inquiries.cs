using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateListingApp.Models
{
    public class Inquiries
    {
        [Key]
        public int InquiryId { get; set; }

        public int UserId { get; set; }

        public int AgentId { get; set; }

        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Please Enter Message...")]
        public string Message { get; set; }

        public DateTime Dates { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("AgentId")]
        public virtual User Agent { get; set; }

        [ForeignKey("PropertyId")]
        public virtual Properties Properties { get; set; }

       
    }
}
