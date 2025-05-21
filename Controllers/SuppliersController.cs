using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.DTOs;
using StockAPI.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SuppliersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(s => s.SupplierId == id);

            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }

        // POST: api/Suppliers
        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(SupplierDto dto)
        {
            var newId = await _context.Suppliers.Select(p => p.SupplierId).DefaultIfEmpty().MaxAsync() + 1;

            var supplier = new Supplier
            {
                SupplierId = newId,
                SupplierName = dto.SupplierName,
                Contact = dto.Contact,
                Email = dto.Email
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSupplier), new { id = supplier.SupplierId }, supplier);
        }

        // PUT: api/Suppliers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            supplier.SupplierName = dto.SupplierName;
            supplier.Contact = dto.Contact;
            supplier.Email = dto.Email;

            _context.Entry(supplier).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists(int id) =>
            _context.Suppliers.Any(e => e.SupplierId == id);
    }
}
