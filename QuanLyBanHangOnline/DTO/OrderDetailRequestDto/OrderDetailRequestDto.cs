using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.OrderDetailRequestDto
{
    // DTO cho từng món hàng trong đơn
    public class OrderDetailRequestDto
    {
        [Required]
        public int IdSP { get; set; }
        public int Quantity { get; set; }
    }
}
