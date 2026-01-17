using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Users;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(PaginationParams @params);
        Task<UserDto?> GetByIdAsync(int id);
        Task CreateAsync(CreateUserDto dto);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}