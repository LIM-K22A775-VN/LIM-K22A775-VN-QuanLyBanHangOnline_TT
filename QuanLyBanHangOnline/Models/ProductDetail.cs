using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QuanLyBanHangOnline.Constants.Enums;

namespace quanlybanhangonline.Models
{
    public class ProductDetail
    {
        [Key]
        [ForeignKey("Product")] // Ràng buộc khóa ngoại tới bảng Product
        public int IdSP { get; set; }

        public ProductSize Size { get; set; }
        public ProductColor Color { get; set; }
        public string Description { get; set; }
        public decimal StartTB { get; set; }

        // Navigation property (giúp truy vấn dữ liệu dễ dàng hơn)
        public virtual Product Product { get; set; }
    }
}
