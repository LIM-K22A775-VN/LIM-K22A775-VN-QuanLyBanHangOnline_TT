using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Staffs;
using QuanLyBanHangOnline.Services.Interfaces;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.Helpers;

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
        public async Task<PagedResult<StaffDto>> GetAllAsync(PaginationParams @params)
        {
            var query = _context.Staff
                .Include(s => s.Role)
                .OrderBy(s => s.IdAccount)
                .Select(s => new StaffDto
                {
                    IdStaff = s.IdAccount,
                    FullName = s.FullName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address,
                    Salary = s.Salary,
                    RoleId = s.RoleId,
                    RoleName = s.Role != null ? s.Role.Title : "Chưa gán quyền" // Hiển thị tên quyền
                }) ;

            // 5. Trả về PagedResult dùng chung cho toàn bộ dự án
            return await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);
        }

        // Lấy thông tin chi tiết một nhân viên theo ID
        public async Task<StaffDto?> GetByIdAsync(int id)
        {
            var staff = await _context.Staff.Include(s => s.Role).FirstOrDefaultAsync(s => s.IdAccount == id);
            if (staff == null) return null;

            return new StaffDto
            {
                IdStaff = staff.IdAccount,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Address = staff.Address,
                Salary = staff.Salary,
                RoleId = staff.RoleId,
                RoleName = staff.Role?.Title ?? "Chưa gán quyền"
            };
        }

        // Tạo mới nhân viên và mã hóa mật khẩu
        public async Task CreateAsync(CreatStaffDto dto)
        {
            // 1. Kiểm tra xem Email đã tồn tại trong hệ thống chưa
            var emailExists = await _context.Staff.AnyAsync(s => s.Email == dto.Email);
            if (emailExists)
            {
                //  Custom Exception hoặc ném Exception thông thường
                throw new Exception("Email này đã được sử dụng bởi một nhân viên khác.");
            }
            // 2. Nếu chưa tồn tại, tiến hành tạo mới
            var staff = new Staff
            {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? "Chưa cập nhật",
                Phone = dto.Phone ?? "Chưa cập nhật",
                Address = dto.Address ?? "Chưa cập nhật",
                Salary = dto.Salary ?? 0,
                RoleId = dto.RoleId
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

            if (dto.RoleId.HasValue)
            {
                // Kiểm tra RoleId có thực sự tồn tại trong DB không
                var roleExists = await _context.Role.AnyAsync(r => r.Id == dto.RoleId.Value);
                if (!roleExists) throw new Exception("Nhóm quyền được chọn không tồn tại.");

                staff.RoleId = dto.RoleId.Value;
            }

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