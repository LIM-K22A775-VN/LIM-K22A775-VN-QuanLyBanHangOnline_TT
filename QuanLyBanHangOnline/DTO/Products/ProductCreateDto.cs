using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
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
