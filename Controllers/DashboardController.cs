using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var totalPurchaseAmount = _context.Purchases
            .AsEnumerable()
               .Sum(p => p.Total);

            var totalSaleAmount = _context.Sales
                .AsEnumerable()
                .Sum(s => s.Total);


            var result = new
            {

                TotalPurchaseAmount = totalPurchaseAmount,
                TotalSaleAmount = totalSaleAmount
            };

            return Ok(result);
        }
    }
}