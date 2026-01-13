using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace QuanLyBanHangOnline.DTO.Orders
{
    public class OrderResponseDto
    {
        public int IdDH { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Chuyển Enum thành chữ "Chờ xác nhận"
        public string CustomerName { get; set; } // Tên khách hàng

        // Danh sách các món hàng đã mua
        public List<OrderDetailResponseDto> Items { get; set; }
    }
}
