using QuanLyBanHangOnline.DTO.Revenue;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IRevenueService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);

    }
}
