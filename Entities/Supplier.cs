using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAPI.Entities
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Contact { get; set; }


        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}
