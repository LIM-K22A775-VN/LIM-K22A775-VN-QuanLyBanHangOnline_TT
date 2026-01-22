using QuanLyBanHangOnline.DTO.Revenue;

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }    // Tổng tiền bán được
    public decimal TotalImportCost { get; set; } // Tổng tiền nhập hàng (Mới)
    public decimal Profit => TotalRevenue - TotalImportCost; // Lợi nhuận (Mới)
    public int TotalOrders { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<OrderSummaryDto> Orders { get; set; } = new List<OrderSummaryDto>();
}