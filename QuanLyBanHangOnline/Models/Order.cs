using QuanLyBanHangOnline.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace quanlybanhangonline.Models
{
    // Model Đơn hàng
    public class Order
    {
        [Key]
        public int IdDH { get; set; }

        [Required]
        public int IdUser { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now; // Gán mặc định thời gian hiện tại
        public decimal TotalPrice { get; set; }

        public Enums.OrderStatus Status { get; set; } = Enums.OrderStatus.ChoXacNhan; // Mặc định là chờ xác nhận

        public string ReceiverName { get; set; } // Tên người nhận

        public string ReceiverPhone { get; set; } // SĐT người nhận
        public string ShippingAddress { get; set; } // Địa chỉ giao hàng
        public string? OrderNotes { get; set; } // Ghi chú đơn hàng (ví dụ: giao giờ hành chính)


        [ForeignKey("IdUser")]
        public virtual User? User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}


