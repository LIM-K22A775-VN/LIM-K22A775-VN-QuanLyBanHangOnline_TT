using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.Services.Interfaces; // Kiểm tra namespace cho chuẩn

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class AppAuthorizationService : IAppAuthorizationService
    {
        private readonly ApplicationDbContext _context;

        public AppAuthorizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckPermissionAsync(int roleId, string permissionSlug)
        {
            // 1. Truy vấn trực tiếp vào DB để lấy chuỗi JSON Permissions mới nhất
            var role = await _context.Role
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null || string.IsNullOrEmpty(role.Permissions)) return false;

            // 2. Giải mã JSON (Ví dụ: từ "["staff_view"]" sang List)
            var permissions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(role.Permissions);

            // 3. Kiểm tra xem quyền yêu cầu có nằm trong danh sách không
            return permissions != null && permissions.Contains(permissionSlug);
        }
    }
}