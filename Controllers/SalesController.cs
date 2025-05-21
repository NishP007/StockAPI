using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.DTOs;
using StockAPI.Entities;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<IActionResult> GetAllSales()
        {
            var sales = await _context.Sales
               
                .Include(sd => sd.Product)
                .ToListAsync();
            return Ok(sales);
        }

        // GET: api/Sales/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSale(int id)
        {
            var sale = await _context.Sales
                
                .Include(sd => sd.Product)
                .FirstOrDefaultAsync(s => s.SaleId == id);

            if (sale == null)
                return NotFound(new { message = $"Sale with ID {id} not found." });

            return Ok(sale);
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return BadRequest(new { message = "Product not found." });

            if (product.QuantityInStock < dto.Quantity)
                return BadRequest(new { message = "Insufficient stock." });

           

            product.QuantityInStock -= dto.Quantity;


            int newSaleId = await _context.Sales.Select(s => s.SaleId).DefaultIfEmpty().MaxAsync() + 1;

            var sale = new Sale
            {
                SaleId = newSaleId,
                SaleDate = dto.SaleDate,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
               
            };
            _context.Sales.Add(sale);

          
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSale), new { id = sale.SaleId }, sale);
        }

        // PUT: api/Sales/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleDTO dto)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.SaleId == id);
            if (sale == null)
                return NotFound(new { message = "Sale not found." });

            var oldDetail = sale;
            if (oldDetail == null)
                return NotFound(new { message = "Sale detail missing." });

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return BadRequest(new { message = "Product not found." });

            // Revert old stock
            product.QuantityInStock += oldDetail.Quantity;

            // Check new stock availability
            if (product.QuantityInStock < dto.Quantity)
                return BadRequest(new { message = "Insufficient stock for update." });

            // Update stock
            product.QuantityInStock -= dto.Quantity;

            // Update sale and detail
            sale.SaleDate = dto.SaleDate;
          

            oldDetail.ProductId = dto.ProductId;
            oldDetail.Quantity = dto.Quantity;
            oldDetail.UnitPrice = dto.UnitPrice;

       
            await _context.SaveChangesAsync();
            return Ok(sale);
        }

        // DELETE: api/Sales/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FirstOrDefaultAsync(s => s.SaleId == id);
            if (sale == null)
                return NotFound(new { message = "Sale not found." });

       
           
            _context.Sales.Remove(sale);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Sale deleted and stock restored." });
        }
    }
}
