using System.ComponentModel.DataAnnotations;
using static QuanLyBanHangOnline.Constants.Enums;

namespace quanlybanhangonline.Models
{
    // Model Sản phẩm
    public class Product
    {
        [Key] // Xác định đây là khóa chính để có thể tạo Controller
        public int IdSP { get; set; } // Sửa từ Id thành IdSP cho đúng ảnh phác thảo

        [Required] // Bắt buộc nhập tên sản phẩm
        public string Name { get; set; } // Tên

        public decimal Price { get; set; } // Giá

        public int StockQuantity { get; set; } // SL H/á (Số lượng hàng hóa)

        public double AverageRating { get; set; }

        public ProductCategory Category { get; set; } // Dmuc (Danh mục)
        public string Image { get; set; } // Dmuc (Danh mục)

        // Liên kết 1-1: Một sản phẩm có một chi tiết
        public virtual ProductDetail ProductDetail { get; set; }
    }
}