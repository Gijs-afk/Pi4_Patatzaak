using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi4_Patatzaak.Models
{
    public class Discount
    {
        [Key]
        public int DiscountID { get; set; }
        public int ProductID { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPrice { get; set; }
        public Product Product { get; set; }
    }
}
