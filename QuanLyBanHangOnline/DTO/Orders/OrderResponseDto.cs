using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace QuanLyBanHangOnline.DTO
{
    public class OrderResponseDto
    {
        public int IdDH { get; set; }
        public int IdUser { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } // Ví dụ: "ChoXacNhan", "DaHuy"...

        // --- BỔ SUNG CÁC TRƯỜNG THÔNG TIN GIAO HÀNG ---
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string? OrderNotes { get; set; }

        // Danh sách các món hàng đã mua
        public List<OrderDetailResponseDto> Items { get; set; } = new List<OrderDetailResponseDto>();
    }
}