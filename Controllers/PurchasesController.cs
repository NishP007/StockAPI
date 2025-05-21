using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.DTOs;
using StockAPI.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/purchases
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();

            return Ok(purchases);
        }

        // GET: api/purchases/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
                return NotFound(new { message = $"Purchase with ID {id} not found." });

            return Ok(purchase);
        }

        // POST: api/purchases
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId);
            if (!supplierExists)
                return BadRequest(new { serverError = "Supplier does not exist." });

            //var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);
            //if (!productExists)
            //    return BadRequest(new { serverError = "Product does not exist." });

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return BadRequest(new { message = "Product not found." });

            if (product.Price != dto.UnitPrice)
            {
                product.Price = dto.UnitPrice;
                _context.Products.Update(product);
            }

            product.QuantityInStock += dto.Quantity;
           

            var newId = await _context.Purchases.Select(p => p.PurchaseId).DefaultIfEmpty().MaxAsync() + 1;

            var purchase = new Purchase
            {
                PurchaseId = newId,
                PurchaseDate = dto.PurchaseDate,
                SupplierId = dto.SupplierId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            _context.Purchases.Add(purchase);

            // Optional: Update product stock quantity
            //var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
            //if (product != null)
            //{
            //    product.QuantityInStock += dto.Quantity;
            //}

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = purchase.PurchaseId }, purchase);
        }

        // PUT: api/purchases/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] PurchaseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase == null)
                return NotFound(new { message = $"Purchase with ID {id} not found." });

            var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId);
            if (!supplierExists)
                return BadRequest(new { serverError = "Supplier does not exist." });

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);
            if (!productExists)
                return BadRequest(new { serverError = "Product does not exist." });

            // Adjust product stock if quantity or product changed (optional)
            if (purchase.ProductId != dto.ProductId || purchase.Quantity != dto.Quantity)
            {
                var oldProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == purchase.ProductId);
                var newProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

                if (oldProduct != null)
                    oldProduct.QuantityInStock -= purchase.Quantity;

                if (newProduct != null)
                    newProduct.QuantityInStock += dto.Quantity;
            }

            purchase.PurchaseDate = dto.PurchaseDate;
            purchase.SupplierId = dto.SupplierId;
            purchase.ProductId = dto.ProductId;
            purchase.Quantity = dto.Quantity;
            purchase.UnitPrice = dto.UnitPrice;

            await _context.SaveChangesAsync();

            return Ok(purchase);
        }

        // DELETE: api/purchases/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase == null)
                return NotFound(new { message = $"Purchase with ID {id} not found." });

            // Optional: Rollback stock if product exists
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == purchase.ProductId);
            if (product != null)
            {
                product.QuantityInStock -= purchase.Quantity;
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Purchase with ID {id} deleted successfully." });
        }
    }
}
