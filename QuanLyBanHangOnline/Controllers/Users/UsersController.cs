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
using QuanLyBanHangOnline.Helpers;
using QuanLyBanHangOnline.Infrastructure.Jwt;

namespace QuanLyBanHangOnline.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly JwtUtils _jwtUtils;
        private readonly ApplicationDbContext _context; // Thêm DbContext
        public UsersController(IUserService userService, IAppAuthorizationService authService, JwtUtils jwtUtils, ApplicationDbContext context) : base(authService)
        {
            _userService = userService;
            _jwtUtils = jwtUtils;
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> GetUser([FromQuery] PaginationParams @params)
        {
            // Kiểm tra quyền xem danh sách khách hàng
            if (!await HasPermission("user_view"))
            {
                return Forbid();
            }

            var users = await _userService.GetAllAsync(@params);
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // Logic: Chính khách hàng đó xem mình HOẶC nhân viên có quyền "user_view"
            bool isOwnerOrAdmin = IsOwnerOrAdmin(id);
            bool hasViewPermission = await HasPermission("user_view");

            if (!isOwnerOrAdmin && !hasViewPermission) return Forbid();
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UpdateUserDto dto)
        {
            // Logic: Chính chủ sửa mình HOẶC nhân viên có quyền "user_edit"
            bool isOwnerOrAdmin = IsOwnerOrAdmin(id);
            bool hasEditPermission = await HasPermission("user_edit");

            if (!isOwnerOrAdmin && !hasEditPermission) return Forbid();
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
                // 1. Tạo User mới thông qua Service
                var user = await _userService.CreateAsync(dto);

                // 2. Tạo cặp Token và gán vào Object user thông qua JwtUtils
                var response = await _jwtUtils.GenerateSignInResponse(user.IdUser, user.Email, "User", user, 0);

                // 3. Lưu Refresh Token vào Database
                await _context.SaveChangesAsync();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
            // DELETE: api/Users/5
            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Chỉ những người có quyền "user_delete" mới được xóa khách hàng
            if (!await HasPermission("user_delete"))
            {
                return Forbid();
            }

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
