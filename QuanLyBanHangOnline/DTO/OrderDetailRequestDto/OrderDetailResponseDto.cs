namespace QuanLyBanHangOnline.DTO.OrderDetailRequestDto
{
    public class OrderDetailResponseDto
    {
        public int IdOrderDetail { get; set; }
        public int IdSP { get; set; }
        public string ProductName { get; set; } // Trả về tên SP cho dễ xem
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal => Quantity * Price; // Thành tiền từng món
    }


}
