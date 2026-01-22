using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Auth;
using QuanLyBanHangOnline.DTO.Profile;
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
                    return Ok(new { id = admin.IdAccount, email = admin.Email, role = "Admin", fullName = "Quản trị viên" });

                case "Staff":
                    var staff = await _context.Staff.FindAsync(userId);
                    if (staff == null) return NotFound("Không tìm thấy thông tin nhân viên");
                    return Ok(new StaffDto
                    {
                        IdStaff = staff.IdAccount,
                        FullName = staff.FullName,
                        Email = staff.Email,
                        Phone = staff.Phone,
                        Address = staff.Address,
                        RoleId = staff.RoleId,
                    });

                case "User":
                    var user = await _context.User.FindAsync(userId);
                    if (user == null) return NotFound("Không tìm thấy thông tin người dùng");
                    return Ok(new { id = user.IdAccount, email = user.Email, fullName = user.FullName, role = "User" , address = user.Address , phone = user.Phone });

                default:
                    return BadRequest("Role không hợp lệ");
            }
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            // 1. Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Mật khẩu mới và xác nhận mật khẩu không khớp.");

            // 2. Lấy Id và Role từ Token người dùng đang đăng nhập
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 3. Tìm tài khoản dựa trên Role
            object? account = null;
            switch (userRole)
            {
                case "Admin": account = await _context.Admin.FindAsync(userId); break;
                case "Staff": account = await _context.Staff.FindAsync(userId); break;
                case "User": account = await _context.User.FindAsync(userId); break;
            }

            if (account == null) return NotFound("Tài khoản không tồn tại.");

            // 4. Kiểm tra mật khẩu cũ (Sử dụng Reflection để lấy thuộc tính Password)
            var passwordProp = account.GetType().GetProperty("Password");
            var currentHashedPassword = passwordProp?.GetValue(account) as string;

            if (string.IsNullOrEmpty(currentHashedPassword) ||
                !BCrypt.Net.BCrypt.Verify(dto.OldPassword, currentHashedPassword))
            {
                return BadRequest("Mật khẩu cũ không chính xác.");
            }

            // 5. Mã hóa mật khẩu mới và lưu vào database
            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            passwordProp?.SetValue(account, newHashedPassword);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đổi mật khẩu thành công!" });
        }


        // API: Cập nhật thông tin cá nhân (Tên, SĐT, Địa chỉ)
        [HttpPut("update-me")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            switch (userRole)
            {
                case "Admin":
                    var admin = await _context.Admin.FindAsync(userId);
                    if (admin == null) return NotFound();
                    // Admin thường chỉ có Email, nếu có FullName hãy update ở đây
                    break;

                case "Staff":
                    var staff = await _context.Staff.FindAsync(userId);
                    if (staff == null) return NotFound();
                    staff.FullName = dto.FullName;
                    staff.Phone = dto.Phone;
                    staff.Address = dto.Address;
                    break;

                case "User":
                    var user = await _context.User.FindAsync(userId);
                    if (user == null) return NotFound();
                    user.FullName = dto.FullName;
                    user.Phone = dto.Phone;
                    user.Address = dto.Address;
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thông tin cá nhân thành công!" });
        }
    }
}