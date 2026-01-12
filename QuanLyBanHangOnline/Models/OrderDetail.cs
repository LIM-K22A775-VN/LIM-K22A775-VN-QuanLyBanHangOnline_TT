using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class OrderDetail
    {
        [Key] // Khóa chính riêng biệt giúp dễ dàng add Controller
        public int IdOrderDetail { get; set; }

        [Required]
        public int IdDH { get; set; } // Giờ chỉ là khóa ngoại trỏ về đơn hàng

        [Required]
        public int IdSP { get; set; } // Giờ chỉ là khóa ngoại trỏ về sản phẩm

        public int Quantity { get; set; } // SL (Số lượng)

        public decimal Price { get; set; } // Giá

        // Thiết lập mối quan hệ để dễ dàng lấy dữ liệu kèm theo (Join)
        [ForeignKey("IdDH")]
        public virtual Order Order { get; set; }

        [ForeignKey("IdSP")]
        public virtual Product Product { get; set; }
    }
}