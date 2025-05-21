using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StockAPI.DTOs
{
    public class SaleDTO
    {
        public DateTime SaleDate { get; set; }

     
        public int Quantity { get; set; }
  
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;


        public int ProductId { get; set; }

    }
}
