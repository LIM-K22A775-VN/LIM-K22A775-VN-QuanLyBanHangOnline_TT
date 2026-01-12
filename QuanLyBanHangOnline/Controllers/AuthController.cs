using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyBanHangOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {


            var admin = await _context.Admin
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (admin != null && BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
                return Ok(new { token = GenerateJwtToken(admin.IdAdmin, admin.Email, "Admin") });

            var staff = await _context.Staff
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (staff != null && BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password))
                return Ok(new { token = GenerateJwtToken(staff.IdStaff, staff.Email, "Staff") });

            var user = await _context.User
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Ok(new { token = GenerateJwtToken(user.IdUser, user.Email, "User") });

            return Unauthorized("Sai email hoặc mật khẩu");
        }


        private string GenerateJwtToken(int id, string email, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpireMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
