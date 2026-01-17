namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductResponseDto
    {
        // Thuộc tính từ bảng Product
        public int IdSP { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }

        // Thuộc tính bổ sung từ bảng ProductDetail
        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal StartTB { get; set; } // Điểm đánh giá trung bình
    }
}