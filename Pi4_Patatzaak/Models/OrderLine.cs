﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pi4_Patatzaak.Models
{
    public class OrderLine
    {
        [Key]
        public int OrderLineID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualPrice { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
 