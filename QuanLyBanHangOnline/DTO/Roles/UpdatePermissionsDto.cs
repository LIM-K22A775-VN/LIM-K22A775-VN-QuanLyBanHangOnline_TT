namespace QuanLyBanHangOnline.DTO.Role
{
    public class UpdatePermissionsDto
    {
        /// <summary>
        /// Danh sách các mã quyền được tích chọn từ ma trận.
        /// </summary>
        /// <example>["product_view", "product_create", "category_edit", "role_permissions"]</example>
        public List<string> Permissions { get; set; } = new List<string>();
    }
}