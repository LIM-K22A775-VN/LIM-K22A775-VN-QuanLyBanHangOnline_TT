using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int StockQuantity { get; set; }

        public string? Category { get; set; }

        // Dùng IFormFile để nhận file ảnh từ client
        public IFormFile? ImageFile { get; set; }

        // Trường cho bảng ProductDetail
        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
}
