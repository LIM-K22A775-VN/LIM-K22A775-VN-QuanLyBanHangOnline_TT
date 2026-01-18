using System.Security.Claims;
using Newtonsoft.Json;

namespace QuanLyBanHangOnline.Helpers
{
    public static class PermissionHelper
    {
        public static bool Check(ClaimsPrincipal user, string requiredPermission)
        {
            // 1. Lấy Role định danh (Admin/Staff/User) từ Token
            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

            // 2. Nếu là Admin -> Full quyền, không cần RoleId
            if (userRole == "Admin") return true;

            // 3. Nếu là Staff -> Bắt đầu xét quyền theo RoleId
            if (userRole == "Staff")
            {
                // Lấy danh sách quyền được đính kèm trong Token lúc Login
                var permissionsJson = user.FindFirst("Permissions")?.Value;

                if (!string.IsNullOrEmpty(permissionsJson))
                {
                    var permissions = JsonConvert.DeserializeObject<List<string>>(permissionsJson);
                    return permissions != null && permissions.Contains(requiredPermission);
                }
            }

            // 4. Nếu là User hoặc các trường hợp khác -> Không có quyền
            return false;
        }
    }
}