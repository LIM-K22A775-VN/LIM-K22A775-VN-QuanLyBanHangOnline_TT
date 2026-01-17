using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Staffs;
namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IStaffService
    {
        Task<PagedResult<StaffDto>> GetAllAsync(PaginationParams @params);
        Task<StaffDto?> GetByIdAsync(int id);
        Task CreateAsync(CreatStaffDto dto);
        Task<bool> UpdateAsync(int id, UpdateStaffDto dto);
        Task<bool> DeleteAsync(int id);
    }
}