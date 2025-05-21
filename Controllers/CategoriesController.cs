using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using StockAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace StockAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _databaseContext;

        public CategoriesController(AppDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var r = await _databaseContext.Categories.Include(x => x.Products).Select(x => new { CategoryId = x.CategoryId, CategoryName = x.CategoryName, pName = x.Products }).ToListAsync();

            return Ok(r);
        }

        // GET: api/designation/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var designation = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (designation == null)
                return NotFound(new { message = $"Designation with ID {id} not found." });

            return Ok(designation);
        }

        // POST: api/designation
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryDto designation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var designationName = designation.CategoryName.Trim().ToUpper();

            // Check if the designation already exists
            var isExist = await _databaseContext.Categories.AnyAsync(x => x.CategoryName == designationName);
            if (isExist)
                return BadRequest(new { serverError = "Designation already exists." });

            // Generate new ID manually
            var newId = await _databaseContext.Categories.Select(x => x.CategoryId).DefaultIfEmpty().MaxAsync() + 1;

            var designationEntity = new Category
            {
                CategoryId = newId,
                CategoryName = designationName,

            };

            _databaseContext.Categories.Add(designationEntity);
            await _databaseContext.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = designationEntity.CategoryId }, designationEntity);
        }

        // PUT: api/designation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoryDto designation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var designationName = designation.CategoryName.Trim().ToUpper();

            var existingDesignation = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingDesignation == null)
                return NotFound(new { message = $"Categories with ID {id} not found." });

            var isDuplicate = await _databaseContext.Categories
                .AnyAsync(x => x.CategoryName == designationName && x.CategoryId != id);

            if (isDuplicate)
                return BadRequest(new { serverError = "Another designation with the same name already exists." });

            existingDesignation.CategoryName = designationName;
            await _databaseContext.SaveChangesAsync();

            return Ok(existingDesignation);
        }

        // DELETE: api/designation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var designation = await _databaseContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (designation == null)
                return NotFound(new { message = $"Designation with ID {id} not found." });

            _databaseContext.Categories.Remove(designation);
            await _databaseContext.SaveChangesAsync();

            return Ok(new { message = $"Designation with ID {id} deleted successfully." });
        }

        [HttpGet("LOOP")]
        public async Task<IActionResult> LOOP()
        {
            var newId = await _databaseContext.Categories.Select(x => x.CategoryId).DefaultIfEmpty().MaxAsync() + 1;

            for (int i = newId; i < 100000; i++)
            {
                var designationEntity = new Category
                {
                    CategoryId = i,
                    CategoryName = "category" + i.ToString(),

                };
                _databaseContext.Categories.Add(designationEntity);

            }

            await _databaseContext.SaveChangesAsync();

            return Ok();
        }


    }
}