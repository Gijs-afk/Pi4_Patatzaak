using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        [BindNever]
        public virtual Customer? Customer { get; set; }
        public virtual List<OrderLine>? Orderlines { get; set; } 


}
}


