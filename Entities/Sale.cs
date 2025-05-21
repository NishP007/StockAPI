using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StockAPI.Entities
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SaleId { get; set; }

        public DateTime SaleDate { get; set; }

        [Required]
        [Precision(18, 2)]
        public int Quantity { get; set; }
        [Required]
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

       
        public decimal Total => Quantity * UnitPrice;


        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

      
    }
}
