using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace quanlybanhangonline.Models.DTOs
{
    public class OrderRequestDto
    {
        // Danh sách nhiều sản phẩm
        public List<OrderDetailRequestDto> Items { get; set; } = new List<OrderDetailRequestDto>();

    }
}