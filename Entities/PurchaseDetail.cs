using Microsoft.EntityFrameworkCore;
using StockAPI.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAPI.Entities
{
    public class PurchaseDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int PurchaseDetailId { get; set; }

        //public int PurchaseId { get; set; }

        //[ForeignKey("PurchaseId")]
        //public Purchase Purchase { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        [Precision(18, 2)]
        public decimal Total => Quantity * UnitPrice;
    }
}
