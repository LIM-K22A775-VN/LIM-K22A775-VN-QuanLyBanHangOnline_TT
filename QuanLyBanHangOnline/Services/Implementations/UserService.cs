using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Users;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.Helpers;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UserDto>> GetAllAsync(PaginationParams @params)
        {
            var query = _context.User
                .OrderBy(u => u.IdUser)
                .Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Address = u.Address
                });

            // 2. Sử dụng hàm Helper Extension để xử lý toàn bộ logic phân trang
            return await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                IdUser = user.IdUser,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address
            };
        }

        public async Task CreateAsync(CreateUserDto dto)
        {
            // 1. Kiểm tra xem Email đã tồn tại trong hệ thống chưa
            var emailExists = await _context.User.AnyAsync(s => s.Email == dto.Email);
            if (emailExists)
            {
                //  Custom Exception hoặc ném Exception thông thường
                throw new Exception("Email này đã được sử dụng bởi một người dùng khác.");
            }
            // 2. Nếu chưa tồn tại, tiến hành tạo mới

            var user = new User
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? "",
                Phone = dto.Phone ?? "",
                Address = dto.Address ?? ""
            };
            _context.User.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return false;

            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}