using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.Auth
{
    public class LoginDto
    {
        /// <example>nxtql99@gmail.com</example>
        public string Email { get; set; }
        /// <example>123456</example>
        public string Password { get; set; }
    }
}
