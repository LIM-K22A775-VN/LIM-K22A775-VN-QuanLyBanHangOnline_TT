using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyBanHangOnline.Infrastructure.Jwt
{
    public class JwtUtils
    {
        private readonly IConfiguration _configuration;
        public JwtUtils(IConfiguration configuration)
        { 
            _configuration = configuration; //"Toàn cục"
        }
        //Logic tạo Access Token.
        public string GenerateJwtToken(int id, string email, string role)
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

        //Logic tạo chuỗi ngẫu nhiên cho Refresh Token.
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        //Logic giải mã token cũ.
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
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