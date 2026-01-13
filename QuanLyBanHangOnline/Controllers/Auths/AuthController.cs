using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QuanLyBanHangOnline.DTO.Auth;

namespace QuanLyBanHangOnline.Controllers.Auths
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
            // 1. Kiểm tra Admin
            var admin = await _context.Admin.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (admin != null && BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            {
                return await SignInResponse(admin.IdAdmin, admin.Email, "Admin", admin);
            }

            // 2. Kiểm tra Staff
            var staff = await _context.Staff.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (staff != null && BCrypt.Net.BCrypt.Verify(dto.Password, staff.Password))
            {
                return await SignInResponse(staff.IdStaff, staff.Email, "Staff", staff);
            }

            // 3. Kiểm tra User
            var user = await _context.User.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return await SignInResponse(user.IdUser, user.Email, "User", user);
            }

            return Unauthorized("Sai email hoặc mật khẩu");
        }

        // Hàm phụ để xử lý lưu Token vào DB và trả về kết quả
        private async Task<IActionResult> SignInResponse(int id, string email, string role, object entity)
        {
            var accessToken = GenerateJwtToken(id, email, role);
            var refreshToken = GenerateRefreshToken();

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
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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
            //chìa khóa bí mật (Secret Key)
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
            var principal = GetPrincipalFromExpiredToken(model.AccessToken);
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var idClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (email == null || idClaim == null) return BadRequest("Invalid token");

            // 2. Tìm User trong DB dựa trên Role
            object entity = null;
            if (role == "Admin") entity = await _context.Admin.FirstOrDefaultAsync(u => u.Email == email);
            else if (role == "Staff") entity = await _context.Staff.FirstOrDefaultAsync(u => u.Email == email);
            else entity = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (entity == null) return BadRequest("User not found");

            // 3. Kiểm tra RefreshToken có khớp và còn hạn không
            var dbRefreshToken = (string)entity.GetType().GetProperty("RefreshToken")?.GetValue(entity);
            var dbExpiry = (DateTime?)entity.GetType().GetProperty("RefreshTokenExpiryTime")?.GetValue(entity);

            if (dbRefreshToken != model.RefreshToken || dbExpiry <= DateTime.UtcNow)
                return BadRequest("Invalid or expired refresh token");

            // 4. Cấp cặp token mới
            return await SignInResponse(int.Parse(idClaim), email, role, entity);
        }

        // Hàm phụ để đọc Token đã hết hạn
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            //TokenValidationParameters
            //"Bộ tiêu chí" mà bạn thiết lập
            //để Server đối chiếu mỗi khi có
            //một Token gửi đến.Nếu Token
            //không đáp ứng được dù chỉ một
            //quy tắc trong này, nó sẽ bị coi
            //là không hợp lệ(Unauthorized).
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, //Tắt kiểm tra nguồn gốc
                    ValidateIssuer = false,  //  Tắt đối tượng nhận token.
                    ValidateIssuerSigningKey = true,
                    //yêu cầu Server phải dùng cái Secret Key (_configuration["Jwt:Key"])
                    //đang giữ để tính toán lại chữ ký của Token gửi lên
                    //và so sánh với chữ ký có sẵn trên Token đó.
                    //Nếu không khớp:. Hệ thống sẽ quăng lỗi ngay lập tức.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    ValidateLifetime = false, // Quan trọng: Phải tắt để đọc được token hết hạn
                    ClockSkew = TimeSpan.Zero // Token hết hạn là chết ngay lập tức, không đợi thêm 5 phút
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                //Kiểm tra xem một chuỗi Token gửi lên là thật hay giả và trích xuất dữ liệu từ đó.
                return principal;
            }
            catch (Exception ex)
            {
                // Nếu Token giả, sai chữ ký... 
                return null;
            }
        }



    }



}


//ValidateIssuer :"Ai là người phát hành?"Đảm bảo Token được
//cấp bởi đúng Server của bạn chứ không phải một Server lạ.
//ValidateAudience"Dành cho ai dùng?"Đảm bảo Token này được cấp cho ứng dụng của bạn chứ không phải cho một App khác.
//ValidateLifetime"Còn hạn dùng không?"
//ValidateIssuerSigningKey"Chữ ký có đúng không?" dùng Secret Key để đối chiếu
//IssuerSigningKey"Chìa khóa là gì?"	Cung cấp cái chìa khóa bí mật (Secret Key) để Server dùng nó mở khóa và kiểm tra chữ ký.
//Nếu TokenValidationParameters là "Bản nội quy",
//thì JwtSecurityTokenHandler chính là "Ông cảnh sát"
//cầm bản nội quy đó để kiểm tra tấm thẻ Token.
//JwtSecurityTokenHandler có 3 nhiệm vụ quan trọng nhất
//1.Đọc và Giải mã (ReadToken): Chuyển chuỗi Token (dạng string loằng ngoằng)
//thành một đối tượng C# để bạn có thể đọc được thông tin bên trong (như ID, Role).
//2.Xác thực (ValidateToken): Đây là việc mà hàm của bạn đang làm.
//Nó kiểm tra xem Token có bị giả mạo không, có đúng chữ ký không, có đúng nguồn gốc không.
//3.Tạo Token (CreateToken): Khi người dùng đăng nhập thành công,
//chính đối tượng này sẽ ký và tạo ra chuỗi JWT để trả về cho Client.
//Đầu vào: Chuỗi token (string) và bộ quy tắc tokenValidationParameters.
//Quá trình: tokenHandler sẽ "mổ xẻ" chuỗi string đó ra,
//dùng chìa khóa bí mật để kiểm tra tính hợp lệ.
//Đầu ra: Nó trả về principal (thông tin người dùng đã xác thực)
//và securityToken (đối tượng token đã được giải mã).