using System.ComponentModel.DataAnnotations;

namespace quanlybanhangonline.Models
{
    public class Staff
    {
        [Key] // staff id
        public int IdStaff { get; set; }

        [Required]
        public string Email  { get; set; } // tk

        [Required]
        public string Password { get; set; } // mk

        public string FullName { get; set; } // Tên
        public decimal Salary { get; set; }   // Lương
        public string Phone { get; set; }    // SĐT
        public string Address { get; set; }  // Địa chỉ

        // refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}