using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAPI.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        // ✅ Navigation properties (already correct)
       // public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

        public ICollection<Sale> Sale { get; set; } 
    }
}
