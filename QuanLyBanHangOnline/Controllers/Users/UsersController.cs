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
using QuanLyBanHangOnline.Services.Interfaces;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.Services.Implementations;

namespace QuanLyBanHangOnline.Controllers
{
    [Authorize(Roles = "User,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUser([FromQuery] PaginationParams @params)
        {
            var users = await _userService.GetAllAsync(@params);
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            if (!IsOwnerOrAdmin(id)) return Forbid();
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
        {
            if (!IsOwnerOrAdmin(id)) return Forbid();
            var result = await _userService.UpdateAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser(CreateUserDto dto)
        {
            try
            {
                await _userService.CreateAsync(dto);
                return Ok(new { message = "Tạo nguời dùng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        // Hàm phụ kiểm tra quyền chính chủ
        private bool IsOwnerOrAdmin(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            return currentUserRole == "Admin" || currentUserId == id.ToString();
        }
    }
}
