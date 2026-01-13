using System.ComponentModel.DataAnnotations;

namespace quanlybanhangonline.Models
{
    public class User
    {
        [Key] // user id
        public int IdUser { get; set; }

        [Required]
        [EmailAddress] // Kiểm tra định dạng email
        public string Email { get; set; } // tk (email)

        [Required]
        public string Password { get; set; } // mk

        public string FullName { get; set; } // Tên
        public string Phone { get; set; }    // SĐT
        public string Address { get; set; }  // Địa chỉ



        // refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}