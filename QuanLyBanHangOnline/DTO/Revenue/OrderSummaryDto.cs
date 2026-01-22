namespace QuanLyBanHangOnline.DTO.Revenue
{
    public class OrderSummaryDto
    {
        public int IdDH { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
    }
}
