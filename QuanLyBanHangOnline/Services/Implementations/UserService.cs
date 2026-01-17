using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Users;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.User
                .Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Address = u.Address
                }).ToListAsync();
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