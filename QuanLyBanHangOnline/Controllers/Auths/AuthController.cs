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
using QuanLyBanHangOnline.Infrastructure.Gmail;
using QuanLyBanHangOnline.Models;

namespace QuanLyBanHangOnline.Controllers.Auths
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtUtils _jwtUtils;

        public AuthController(ApplicationDbContext context, JwtUtils jwtUtils)
        {
            _context = context;
            _jwtUtils = jwtUtils;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1. Chỉ cần tìm trong bảng Accounts chung (EF sẽ tự Join bảng con nếu cần)
            var account = await _context.Accounts
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (account == null || !BCrypt.Net.BCrypt.Verify(dto.Password, account.Password))
            {
                return Unauthorized("Sai email hoặc mật khẩu");
            }

            // 2. Gọi hàm SignInResponse duy nhất
            return await SignInResponse(account);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy IdAccount từ Token (NameIdentifier)
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest();

            var account = await _context.Accounts.FindAsync(int.Parse(userIdClaim));
            if (account != null)
            {
                account.RefreshToken = null;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel model)
        {
            if (model == null) return BadRequest("Invalid request");

            var principal = _jwtUtils.GetPrincipalFromExpiredToken(model.AccessToken);
            var idClaim = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (idClaim == null) return BadRequest("Invalid token");

            var account = await _context.Accounts.FindAsync(int.Parse(idClaim));

            if (account == null || account.RefreshToken != model.RefreshToken || account.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest("Invalid or expired refresh token");

            return await SignInResponse(account);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, [FromServices] IEmailService emailService)
        {
            // Kiểm tra email trên bảng Accounts chung
            var exists = await _context.Accounts.AnyAsync(x => x.Email == dto.Email);
            if (!exists) return BadRequest("Email không tồn tại.");

            string otpCode = new Random().Next(100000, 999999).ToString();

            var otpEntry = new AccountOtp
            {
                Email = dto.Email,
                OtpCode = otpCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            _context.AccountOtps.Add(otpEntry);
            await _context.SaveChangesAsync();

            await emailService.SendEmailAsync(dto.Email, "Mã xác thực OTP", $"Mã của bạn là: {otpCode}");

            return Ok(new { message = "OTP đã được gửi." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Unauthorized("Thiếu Token xác thực.");

            string token = authHeader.Substring("Bearer ".Length);
            var principal = _jwtUtils.GetPrincipalFromResetToken(token);
            var email = principal?.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email)) return Unauthorized("Token không hợp lệ.");
            if (dto.NewPassword != dto.ConfirmPassword) return BadRequest("Mật khẩu không khớp");

            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (account == null) return NotFound();

            // Cập nhật mật khẩu và xóa OTP
            account.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            var otpsToDelete = await _context.AccountOtps.Where(x => x.Email == email).ToListAsync();
            _context.AccountOtps.RemoveRange(otpsToDelete);

            await _context.SaveChangesAsync();

            return await SignInResponse(account);
        }

        // Hàm phụ tinh gọn sử dụng lớp cha Account
        private async Task<IActionResult> SignInResponse(Account account)
        {
            // JwtUtils sẽ tự xử lý Claims dựa trên RoleType của account
            var accessToken = _jwtUtils.GenerateJwtToken(account);
            var refreshToken = _jwtUtils.GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }

}


