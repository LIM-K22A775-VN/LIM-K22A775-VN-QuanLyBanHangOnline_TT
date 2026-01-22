using System.ComponentModel.DataAnnotations;
using static QuanLyBanHangOnline.Constants.Enums;

namespace QuanLyBanHangOnline.DTO.Products
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public ProductCategory Category { get; set; }

        // Dùng IFormFile để nhận file ảnh từ client
        public IFormFile? ImageFile { get; set; }

        // Trường cho bảng ProductDetail
        public ProductSize Size { get; set; }
        public ProductColor Color { get; set; }
        public string Description { get; set; }
    }
}
