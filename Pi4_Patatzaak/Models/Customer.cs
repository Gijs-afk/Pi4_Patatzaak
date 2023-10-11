using System.ComponentModel.DataAnnotations;

namespace Pi4_Patatzaak.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public List<Order>? Orders { get; set; }

    }
}
