using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Staffs;
using QuanLyBanHangOnline.Services.Interfaces;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _context;

        public StaffService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ danh sách nhân viên và chuyển sang DTO
        public async Task<IEnumerable<StaffDto>> GetAllAsync()
        {
            return await _context.Staff
                .Select(s => new StaffDto
                {
                    IdStaff = s.IdStaff,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    Salary = s.Salary
                }).ToListAsync();
        }

        // Lấy thông tin chi tiết một nhân viên theo ID
        public async Task<StaffDto?> GetByIdAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return null;

            return new StaffDto
            {
                IdStaff = staff.IdStaff,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Address = staff.Address,
                Salary = staff.Salary
            };
        }

        // Tạo mới nhân viên và mã hóa mật khẩu
        public async Task CreateAsync(CreatStaffDto dto)
        {
            var staff = new Staff
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? "Chưa cập nhật",
                Phone = dto.Phone ?? "Chưa cập nhật",
                Address = dto.Address ?? "Chưa cập nhật",
                Salary = dto.Salary ?? 0
            };

            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin nhân viên (có xử lý ghi đè dữ liệu cũ)
        public async Task<bool> UpdateAsync(int id, UpdateStaffDto dto)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;

            staff.FullName = dto.FullName ?? staff.FullName;
            staff.Phone = dto.Phone ?? staff.Phone;
            staff.Address = dto.Address ?? staff.Address;
            staff.Salary = dto.Salary ?? staff.Salary;

            // Nếu người dùng có nhập mật khẩu mới thì mới hash lại
            if (!string.IsNullOrEmpty(dto.Password))
            {
                staff.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Xóa nhân viên khỏi Database
        public async Task<bool> DeleteAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}