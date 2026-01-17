using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace QuanLyBanHangOnline.DTO
{
    public class OrderResponseDto
    {
        public int IdDH { get; set; }
        public int IdUser { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Chuyển Enum thành chữ "Chờ xác nhận"

        // Danh sách các món hàng đã mua
        public List<OrderDetailResponseDto> Items { get; set; }
    }
}
