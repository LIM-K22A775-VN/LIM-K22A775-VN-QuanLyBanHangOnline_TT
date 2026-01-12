using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    // Model Đơn hàng
    public class Order
    {
        [Key]
        public int IdDH { get; set; } // Id-ĐH

        [Required]
        public int IdUser { get; set; } // Id User (Khóa ngoại)
        public DateTime OrderDate { get; set; } // T/g
        public decimal TotalPrice { get; set; } // Tổng tiền
        public string Status { get; set; } // Trang thai


        // Thiết lập mối quan hệ để dễ dàng lấy dữ liệu kèm theo (Join)
        [ForeignKey("IdUser")]
        public virtual User? User { get; set; }
    }
}
