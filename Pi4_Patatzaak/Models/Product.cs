using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi4_Patatzaak.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int? PictureID { get; set; }
        public virtual Pictures? Picture { get; set; }
        public virtual Discount? Discount { get; set; } 
        public virtual List<OrderLine> Orderlines { get; set; }
    }
}
