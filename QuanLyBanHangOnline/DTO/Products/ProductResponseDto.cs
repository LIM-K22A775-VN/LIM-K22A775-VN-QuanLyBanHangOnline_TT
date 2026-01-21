using static QuanLyBanHangOnline.Constants.Enums;

namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductResponseDto
    {
        // Thuộc tính từ bảng Product
        public int IdSP { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public ProductCategory Category { get; set; }
        public string ImageUrl { get; set; }

        // Thuộc tính bổ sung từ bảng ProductDetail
        public ProductSize Size { get; set; }
        public ProductColor Color { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; } // Điểm đánh giá trung bình
    }
}