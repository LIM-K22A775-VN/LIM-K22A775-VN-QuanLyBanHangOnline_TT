using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class Staff
    {
        [Key] // staff id
        public int IdStaff { get; set; }

        [EmailAddress] // Thêm validation cho Email
        [StringLength(255)]
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


        // Liên kết tới bảng Role (Có thể null nếu là Admin)
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}