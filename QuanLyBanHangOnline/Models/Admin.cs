using System.ComponentModel.DataAnnotations;

namespace quanlybanhangonline.Models
{
    public class Admin
    {
        [Key] // Xác định Id là khóa chính
        public int IdAdmin { get; set; }

        [Required] // Bắt buộc phải có
        [EmailAddress] // Kiểm tra định dạng email
        public string Email { get; set; } // email

        [Required]
        public string Password { get; set; } // mk

        // refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}