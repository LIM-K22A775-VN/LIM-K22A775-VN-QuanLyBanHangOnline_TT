using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace quanlybanhangonline.Models.DTOs
{
    public class OrderRequestDto
    {
        // Danh sách nhiều sản phẩm
        public List<OrderDetailRequestDto> Items { get; set; } = new List<OrderDetailRequestDto>();

        // Bạn nên thêm các thông tin giao hàng ở đây để validate luôn
        public string? ReceiverName { get; set; }
        public string? ReceiverPhone { get; set; }
        public string? ShippingAddress { get; set; }

        public string? OrderNotes { get; set; }

    }
}