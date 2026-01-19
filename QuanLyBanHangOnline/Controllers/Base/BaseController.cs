using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyBanHangOnline.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IAppAuthorizationService _authService;

        protected BaseController(IAppAuthorizationService authService)
        {
            _authService = authService;
        }

        // Hàm này sẽ dùng chung cho Product, Order, Staff...
        protected async Task<bool> HasPermission(string slug)
        {
            // 1. Ưu tiên Admin
            if (User.FindFirstValue(ClaimTypes.Role) == "Admin") return true;

            // 2. Lấy RoleId từ Token (Cái này không đổi trong suốt phiên làm việc)
            var roleIdStr = User.FindFirst("RoleId")?.Value;
            if (string.IsNullOrEmpty(roleIdStr)) return false;

            // 3. Truy vấn DB thực tế thông qua Service
            return await _authService.CheckPermissionAsync(int.Parse(roleIdStr), slug);
        }
    }
}