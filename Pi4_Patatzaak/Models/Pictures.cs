using System.ComponentModel.DataAnnotations;

namespace Pi4_Patatzaak.Models
{
    public class Pictures
    {
        [Key]
        public int PictureID { get; set; }
        [Required]
        public string FileName { get; set; }
        public string? PictureDescription { get; set;}
        public virtual List<Product> Products { get; set; }
    }
}
