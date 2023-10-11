using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi4_Patatzaak.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        [ForeignKey("CustomerID")]
        public int CustomerID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = ("Order arived at the shop");
        public Customer Customer { get; set; }
        public List<OrderLine> Orderlines { get; set; }


    }
}
