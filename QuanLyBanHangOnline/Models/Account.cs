using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.Models
{
    public abstract class Account
    {
        [Key]
        public int IdAccount { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Thuộc tính để phân biệt loại tài khoản khi cần
        public string RoleType { get; set; }
    }
}
