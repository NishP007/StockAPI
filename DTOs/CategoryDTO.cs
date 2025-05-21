using System.ComponentModel.DataAnnotations;

namespace StockAPI.DTOs
{
    public class CategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }
    }
}
