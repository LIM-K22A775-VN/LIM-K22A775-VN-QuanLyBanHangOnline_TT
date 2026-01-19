namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IAppAuthorizationService
    {
        Task<bool> CheckPermissionAsync(int roleId, string permissionSlug);
    }
}
