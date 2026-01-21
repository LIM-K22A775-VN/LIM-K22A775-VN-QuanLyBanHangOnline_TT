using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QuanLyBanHangOnline.Constants.Enums;

namespace quanlybanhangonline.Models
{
    public class Review
    {
        [Key] // Khóa chính riêng biệt
        public int IdReview { get; set; }

        [Required]
        public int IdUser { get; set; } // Giờ chỉ đóng vai trò Khóa ngoại

        [Required]

        public int IdSP { get; set; }   // Giờ chỉ đóng vai trò Khóa ngoại

        public StarRating Rating { get; set; } // Đánh giá số sao (ví dụ từ ảnh: Số *)
        public string Comment { get; set; } // Nội dung đánh giá

        // Thuộc tính điều hướng (Navigation Properties) để liên kết bảng
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }

        [ForeignKey("IdSP")]
        public virtual Product Product { get; set; }

    }
}