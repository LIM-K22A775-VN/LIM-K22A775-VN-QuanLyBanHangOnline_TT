using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Staffs;


namespace QuanLyBanHangOnline.Controllers.extensions
{
    [Authorize] // Bắt buộc phải đăng nhập mới dùng được controller này
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            // 1. Lấy Id và Role từ Token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Tùy theo Role mà tìm trong bảng tương ứng
            switch (userRole)
            {
                case "Admin":
                    var admin = await _context.Admin.FindAsync(userId);
                    if (admin == null) return NotFound("Không tìm thấy thông tin Admin");
                    // Bạn có thể tạo AdminDto nếu cần, ở đây mình trả về object ẩn danh cho nhanh
                    return Ok(new { id = admin.IdAdmin, email = admin.Email, role = "Admin", fullName = "Quản trị viên" });

                case "Staff":
                    var staff = await _context.Staff.FindAsync(userId);
                    if (staff == null) return NotFound("Không tìm thấy thông tin nhân viên");
                    return Ok(new StaffDto
                    {
                        IdStaff = staff.IdStaff,
                        FullName = staff.FullName,
                        Email = staff.Email,
                        Phone = staff.Phone,
                        Address = staff.Address
                    });

                case "User":
                    var user = await _context.User.FindAsync(userId);
                    if (user == null) return NotFound("Không tìm thấy thông tin người dùng");
                    return Ok(new { id = user.IdUser, email = user.Email, fullName = user.FullName, role = "User" });

                default:
                    return BadRequest("Role không hợp lệ");
            }
        }
    }
}