using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace quanlybanhangonline.Models
{
    // Model Đơn hàng
    public class Order
    {
        [Key]
        [JsonIgnore] // Dòng này sẽ làm cho "user" biến mất khỏi JSON yêu cầu
        public int IdDH { get; set; }

        [Required]
        [JsonIgnore] // Dòng này sẽ làm cho "user" biến mất khỏi JSON yêu cầu
        public int IdUser { get; set; }
        [JsonIgnore] // Dòng này sẽ làm cho "user" biến mất khỏi JSON yêu cầu
        public DateTime OrderDate { get; set; } = DateTime.Now; // Gán mặc định thời gian hiện tại
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.ChoXacNhan; // Mặc định là chờ xác nhận

        [ForeignKey("IdUser")]
        [JsonIgnore] // Dòng này sẽ làm cho "user" biến mất khỏi JSON yêu cầu
        public virtual User? User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}


public enum OrderStatus
{
    ChoXacNhan = 0,    // Chờ xác nhận
    DaXacNhan = 1,     // Đã xác nhận
    DaVanChuyen = 2,   // Đã vận chuyển
    DaNhanHang = 3,    // Đã nhận hàng
    DaHuy = 4          // (Nên thêm trạng thái Hủy đơn)
}