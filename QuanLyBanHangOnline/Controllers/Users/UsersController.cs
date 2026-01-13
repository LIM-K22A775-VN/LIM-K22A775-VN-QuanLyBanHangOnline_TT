using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using QuanLyBanHangOnline.DTO.Users;
using System.Security.Claims;

namespace QuanLyBanHangOnline.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUser()
        {
          if (_context.User == null)
          {
              return NotFound();
          }
            return await _context.User
        .Select(u => new UserDto
        {
            IdUser = u.IdUser,
            Email = u.Email,
            FullName = u.FullName,
            Phone = u.Phone,
            Address = u.Address
        })
        .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            // Chặn nếu không phải Admin và cũng không phải chính chủ
            if (currentUserRole == "User" && currentUserId != id.ToString())
            {
                return Forbid();
            }

            if (_context.User == null)
          {
              return NotFound();
          }
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

             return new UserDto
    {
        IdUser = user.IdUser,
        Email = user.Email,
        FullName = user.FullName,
        Phone = user.Phone,
        Address = user.Address
    };
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Chỉ chính chủ hoặc Admin mới được sửa
            if (currentUserRole == "User" && currentUserId != id.ToString())
            {
                return Forbid();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(CreateUserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? "",
                Phone = dto.Phone ?? "",
                Address = dto.Address ?? ""
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.User == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.IdUser == id)).GetValueOrDefault();
        }
    }
}
