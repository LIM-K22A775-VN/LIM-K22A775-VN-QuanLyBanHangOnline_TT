using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.Services.Interfaces;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Generic;
using Humanizer;
using QuanLyBanHangOnline.Helpers;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Admin>> GetAllAsync(PaginationParams @params)
        {
            var query = _context.Admin 
                .OrderBy(a => a.IdAccount)
                .AsQueryable();

            // 2. Dùng hàm Helper đã viết để xử lý toàn bộ logic phân trang & chặn số âm
            return await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _context.Admin.FindAsync(id);
        }

        public async Task CreateAsync(Admin admin)
        {
            // 1. Kiểm tra xem Email đã tồn tại trong hệ thống chưa
            var emailExists = await _context.Admin.AnyAsync(s => s.Email == admin.Email);
            if (emailExists)
            {
                //  Custom Exception hoặc ném Exception thông thường
                throw new Exception("Email này đã được sử dụng bởi một quản trị viên khác.");
            }
            // 2. Nếu chưa tồn tại, tiến hành tạo mới


            // Hash mật khẩu trước khi lưu
            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);
            _context.Admin.Add(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Admin admin)
        {
            if (id != admin.IdAccount) return false;

            // Kiểm tra tồn tại trước khi cập nhật
            var existingAdmin = await _context.Admin.AnyAsync(e => e.IdAccount == id);
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