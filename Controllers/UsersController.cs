using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockAPI.Data;
using StockAPI.DTOs;
using StockAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // ✅ GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ✅ POST: api/Users
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(UserDto dto)
        {
            var newId = await _context.Users.Select(u => u.UserId).DefaultIfEmpty().MaxAsync() + 1;

            var user = new User
            {
                UserId = newId,
                Username = dto.Username,
                Password = dto.Password, // You should hash passwords in production
              
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }


        // ✅ PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Username = dto.Username;
            user.Password = dto.Password;
        

            await _context.SaveChangesAsync();
            return Ok("SuccessFully Change");
        }

        // ✅ DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("Delete Change");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(u => u.UserId == id);
        }
    }
}
