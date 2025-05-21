using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.Entities;
using StockAPI.DTOs;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PurchaseDetails
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products
               .Include(p => p.Category)
               .ToListAsync();

            return Ok(products);
        }

        // GET: api/PurchaseDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseDetail>> GetPurchaseDetail(int id)
        {
            var detail = await _context.PurchaseDetails
                .Include(pd => pd.Product)
                .Include(pd => pd.Purchase)
                .FirstOrDefaultAsync(pd => pd.PurchaseDetailId == id);

            if (detail == null) return NotFound();

            return Ok(detail);
        }
        // POST: api/PurchaseDetails
        [HttpPost]
        public async Task<ActionResult<PurchaseDetail>> CreatePurchaseDetail(PurchaseDetailDto dto)
        {
            if (dto.Quantity <= 0 || dto.UnitPrice <= 0)
                return BadRequest("Quantity and UnitPrice must be greater than zero.");

            var purchaseExists = await _context.Purchases.AnyAsync(p => p.PurchaseId == dto.PurchaseId);
            if (!purchaseExists) return BadRequest("Invalid Purchase ID.");

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == dto.ProductId);
            if (!productExists) return BadRequest("Invalid Product ID.");

            var purchaseDetail = new PurchaseDetail
            {
                PurchaseId = dto.PurchaseId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            _context.PurchaseDetails.Add(purchaseDetail);

         

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchaseDetail), new { id = purchaseDetail.PurchaseDetailId }, purchaseDetail);
        }

        // PUT: api/PurchaseDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseDetail(int id, PurchaseDetailDto dto)
        {
            var detail = await _context.PurchaseDetails.FindAsync(id);
            if (detail == null) return NotFound();

            if (dto.Quantity <= 0 || dto.UnitPrice <= 0)
                return BadRequest("Quantity and Unit Price must be greater than 0.");

            var oldTotal = detail.Quantity * detail.UnitPrice;

            detail.PurchaseId = dto.PurchaseId;
            detail.ProductId = dto.ProductId;
            detail.Quantity = dto.Quantity;
            detail.UnitPrice = dto.UnitPrice;

            // Update Purchase total
            var purchase = await _context.Purchases.FindAsync(detail.PurchaseId);
            if (purchase != null)
            {
                var newTotal = dto.Quantity * dto.UnitPrice;
                purchase.TotalAmount = purchase.TotalAmount - oldTotal + newTotal;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseDetailExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/PurchaseDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseDetail(int id)
        {
            var detail = await _context.PurchaseDetails.FindAsync(id);
            if (detail == null) return NotFound();

            // Update total before deleting
            var purchase = await _context.Purchases.FindAsync(detail.PurchaseId);
            if (purchase != null)
            {
                purchase.TotalAmount -= detail.Quantity * detail.UnitPrice;
            }

            _context.PurchaseDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseDetailExists(int id)
        {
            return _context.PurchaseDetails.Any(e => e.PurchaseDetailId == id);
        }
    }
}
