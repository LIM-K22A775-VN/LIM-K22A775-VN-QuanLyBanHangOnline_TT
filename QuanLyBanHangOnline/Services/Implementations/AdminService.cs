using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.Services.Interfaces;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _context.Admin.ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admin.FindAsync(id);
        }

        public async Task CreateAsync(Admin admin)
        {
            // Hash mật khẩu trước khi lưu
            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
            _context.Admin.Add(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Admin admin)
        {
            if (id != admin.IdAdmin) return false;

            // Kiểm tra tồn tại trước khi cập nhật
            var existingAdmin = await _context.Admin.AnyAsync(e => e.IdAdmin == id);
            if (!existingAdmin) return false;

            _context.Entry(admin).State = EntityState.Modified;

            // Nếu mật khẩu được thay đổi, bạn nên hash lại ở đây (tùy logic UI của bạn)
            // Hiện tại mình giữ nguyên logic mặc định của bạn

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            if (admin == null) return false;

            _context.Admin.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}