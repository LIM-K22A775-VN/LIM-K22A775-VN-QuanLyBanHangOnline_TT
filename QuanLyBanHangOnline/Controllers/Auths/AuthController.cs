using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QuanLyBanHangOnline.DTO.Auth;
using QuanLyBanHangOnline.Infrastructure.Jwt;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Models;

namespace QuanLyBanHangOnline.Controllers.Auths
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtUtils _jwtUtils; // Inject JwtUtils thay vì IConfiguration

        public AuthController(ApplicationDbContext context, JwtUtils jwtUtils)
        {
            _context = context;
            _jwtUtils = jwtUtils;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1. Kiểm tra Admin
            var admin = await _context.Admin.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (admin != null && BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            {
                // Admin không có RoleId trong bảng, truyền 0 hoặc một ID xác định
                return await SignInResponse(admin.IdAdmin, admin.Email, "Admin", admin, 0);
            }

            // 2. Kiểm tra Staff
            var staff = await _context.Staff
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (staff != null && BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password))
            {
                return await SignInResponse(staff.IdStaff, staff.Email, "Staff", staff, staff.RoleId ?? 0);
            }

            // 3. Kiểm tra User
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return await SignInResponse(user.IdUser, user.Email, "User", user, 0);
            }

            return Unauthorized("Sai email hoặc mật khẩu");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy Email từ Token đang đăng nhập
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(email)) return BadRequest();

            // Tìm trong bảng tương ứng và xóa RefreshToken
            if (role == "Admin")
            {
                var account = await _context.Admin.FirstOrDefaultAsync(u => u.Email == email);
                if (account != null) account.RefreshToken = null;
            }
            else if (role == "Staff")
            {
                var account = await _context.Staff.FirstOrDefaultAsync(u => u.Email == email);
                if (account != null) account.RefreshToken = null;
            }
            else
            {
                var account = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
                if (account != null) account.RefreshToken = null;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Logged out successfully" });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel model)
        {
            if (model == null) return BadRequest("Invalid request");

            // 1. Giải mã token cũ để lấy Email và Role (kể cả khi đã hết hạn)
            var principal = _jwtUtils.GetPrincipalFromExpiredToken(model.AccessToken);
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (email == null || idClaim == null) return BadRequest("Invalid token");

            // 2. Tìm User trong DB dựa trên Role
            object entity = null;
            if (role == "Admin") entity = await _context.Admin.FirstOrDefaultAsync(u => u.Email == email);
            else if (role == "Staff") entity = await _context.Staff.Include(s => s.Role).FirstOrDefaultAsync(u => u.Email == email);
            else entity = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (entity == null) return BadRequest("User not found");

            // 3. Kiểm tra RefreshToken có khớp và còn hạn không
            var dbRefreshToken = (string)entity.GetType().GetProperty("RefreshToken")?.GetValue(entity);
            var dbExpiry = (DateTime?)entity.GetType().GetProperty("RefreshTokenExpiryTime")?.GetValue(entity);

            if (dbRefreshToken != model.RefreshToken || dbExpiry <= DateTime.UtcNow)
                return BadRequest("Invalid or expired refresh token");


            int roleId = 0;
            if (role == "Staff" && entity is Staff staffEntity)
            {
                roleId = staffEntity.RoleId ?? 0;
            }
            // 4. Cấp cặp token mới
            return await SignInResponse(int.Parse(idClaim), email, role, entity, roleId);
        }

        // Hàm phụ để xử lý lưu Token vào DB và trả về kết quả
        private async Task<IActionResult> SignInResponse(int id, string email, string role, object entity,int roleId)
        {
            var accessToken = _jwtUtils.GenerateJwtToken(id, email, role, roleId);
            var refreshToken = _jwtUtils.GenerateRefreshToken();

            var type = entity.GetType();
            type.GetProperty("RefreshToken")?.SetValue(entity, refreshToken);
            type.GetProperty("RefreshTokenExpiryTime")?.SetValue(entity, DateTime.UtcNow.AddDays(7));

            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

    }
}


