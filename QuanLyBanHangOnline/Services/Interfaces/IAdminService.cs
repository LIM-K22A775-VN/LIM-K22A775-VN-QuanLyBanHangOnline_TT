using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(int id);
        Task CreateAsync(Admin admin);
        Task<bool> UpdateAsync(int id, Admin admin);
        Task<bool> DeleteAsync(int id);
    }
}