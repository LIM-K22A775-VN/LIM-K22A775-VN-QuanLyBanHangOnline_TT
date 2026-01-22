using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : BaseController
    {
        private readonly IRevenueService _revenueService;

        public RevenueController(IRevenueService revenueService, IAppAuthorizationService authService)
            : base(authService)
        {
            _revenueService = revenueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenue([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            // 1. Kiểm tra quyền hạn (slug: revenue_view)
            if (!await HasPermission("revenue_view")) return Forbid();

            // 2. Xử lý thời gian mặc định (nếu không truyền thì lấy trong tháng hiện tại)
            var start = fromDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = toDate ?? DateTime.Now;

            // Đảm bảo thời gian kết thúc là cuối ngày
            end = end.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var report = await _revenueService.GetRevenueReportAsync(start, end);
            return Ok(report);  
        }
    }
}
