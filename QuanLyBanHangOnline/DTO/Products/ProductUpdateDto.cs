using static QuanLyBanHangOnline.Constants.Enums;

namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductUpdateDto
    {
        public int IdSP { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal ImportPrice { get; set; }

        public int StockQuantity { get; set; }
        public ProductCategory Category { get; set; }
        public IFormFile? ImageFile { get; set; } // Chỉ gửi lên nếu muốn đổi ảnh mới
    }
}
