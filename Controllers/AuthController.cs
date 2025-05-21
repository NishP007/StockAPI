
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockAPI.Data;
using StockAPI.DTOs;
using StockAPI.Helpers;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : Controller
    {
        private readonly AppDbContext _databaseContext;
        private readonly IConfiguration _config;


        public AuthController(AppDbContext databaseContext, IConfiguration config)
        {
            _databaseContext = databaseContext;
            _config = config;
        }

      
        ////////////////////////////////////////////////////LOGIN POST////////////////////////////////////////////////
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto loginDto)
        {
            try
            {
                var result = _databaseContext.Users
                    .FirstOrDefault(x => x.Username == loginDto.Username && x.Password == loginDto.Password);
                if (result == null)
                    return NotFound();

                var token = JwtTokenHelper.GenerateToken(loginDto.Username, result.UserId, _config);
                return Ok(new { Token = token });

            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }



    }
}