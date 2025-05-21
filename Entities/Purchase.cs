using Microsoft.EntityFrameworkCore;
using StockAPI.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;

namespace StockAPI.Entities
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PurchaseId { get; set; }

        [Column("date")]
        public DateTime PurchaseDate { get; set; }


        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; }

        [Precision(18, 2)]
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 2)]
        public decimal Total => Quantity * UnitPrice;

        // public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}
