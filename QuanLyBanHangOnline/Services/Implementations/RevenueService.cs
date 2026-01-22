using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO.Revenue;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class RevenueService : IRevenueService
    {
        private readonly ApplicationDbContext _context;

        public RevenueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate)
        {
            // 1. Tính tổng Doanh thu từ các đơn hàng đã nhận
            var orders = await _context.Order
                .Include(o => o.User)
                .Where(o => o.OrderDate >= fromDate
                         && o.OrderDate <= toDate
                         && o.Status == Enums.OrderStatus.DaNhanHang)
                .ToListAsync();

            // 2. Tính tổng Tiền nhập hàng từ bảng Import (Mới)
            var totalImportCost = await _context.Import
                .Where(i => i.ImportDate >= fromDate && i.ImportDate <= toDate)
                .SumAsync(i => i.TotalCost);

            // 3. Tổng hợp tất cả vào DTO trả về
            var report = new RevenueReportDto
            {
                TotalRevenue = orders.Sum(o => o.TotalPrice),
                TotalImportCost = totalImportCost, // Gán giá trị nhập hàng vào đây
                TotalOrders = orders.Count,
                FromDate = fromDate,
                ToDate = toDate,
                Orders = orders.Select(o => new OrderSummaryDto
                {
                    IdDH = o.IdDH,
                    OrderDate = o.OrderDate,
                    CustomerEmail = o.User?.Email ?? "N/A",
                    TotalPrice = o.TotalPrice,
                    ReceiverName = o.ReceiverName
                }).ToList()
            };

            return report;
        }
    }
}
