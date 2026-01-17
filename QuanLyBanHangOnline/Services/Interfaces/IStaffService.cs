using QuanLyBanHangOnline.DTO.Staffs;
namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllAsync();
        Task<StaffDto?> GetByIdAsync(int id);
        Task CreateAsync(CreatStaffDto dto);
        Task<bool> UpdateAsync(int id, UpdateStaffDto dto);
        Task<bool> DeleteAsync(int id);
    }
}