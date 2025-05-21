using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.Entities;
using StockAPI.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productName = productDto.ProductName.Trim().ToUpper();

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId);
            if (!categoryExists)
                return BadRequest(new { serverError = "Category does not exist." });

            var isDuplicate = await _context.Products.AnyAsync(p => p.ProductName == productName);
            if (isDuplicate)
                return BadRequest(new { serverError = "Product already exists." });

            var newId = await _context.Products.Select(p => p.ProductId).DefaultIfEmpty().MaxAsync() + 1;

            var product = new Product
            {
                ProductId = newId,
                ProductName = productName,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                QuantityInStock = productDto.QuantityInStock
            };

            _context.Products.Add(product);

            // Stock movement - initial stock "IN"
           

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = product.ProductId }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productName = productDto.ProductName.Trim().ToUpper();

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (existingProduct == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            var isDuplicate = await _context.Products
                .AnyAsync(p => p.ProductName == productName && p.ProductId != id);

            if (isDuplicate)
                return BadRequest(new { serverError = "Another product with the same name already exists." });

            int quantityDifference = productDto.QuantityInStock - existingProduct.QuantityInStock;

            existingProduct.ProductName = productName;
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.Price = productDto.Price;
            existingProduct.QuantityInStock = productDto.QuantityInStock;

            // Add stock movement if stock changed
        
            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });

            // If product has quantity, log stock movement as OUT for remaining stock
    
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Product with ID {id} deleted successfully." });
        }
    }
}
