namespace StockAPI.DTOs
{
    public class ProductDto
    {
   
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
    }

   
}
