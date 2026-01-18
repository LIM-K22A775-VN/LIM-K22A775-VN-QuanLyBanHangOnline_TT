using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } // "Quản lý nội dung"

        public string? Description { get; set; } // "quản lý nội dung . . ."

        // Lưu danh sách quyền dưới dạng chuỗi JSON: ["products-category_view", ...]
        public string? Permissions { get; set; }

    }
}
