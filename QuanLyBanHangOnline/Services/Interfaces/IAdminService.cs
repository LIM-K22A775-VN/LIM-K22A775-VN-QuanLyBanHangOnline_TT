using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Generic;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IAdminService
    {
        Task<PagedResult<Admin>> GetAllAsync(PaginationParams @params);
        Task<Admin?> GetByIdAsync(int id);
        Task CreateAsync(Admin admin);
        Task<bool> UpdateAsync(int id, Admin admin);
        Task<bool> DeleteAsync(int id);
    }
}