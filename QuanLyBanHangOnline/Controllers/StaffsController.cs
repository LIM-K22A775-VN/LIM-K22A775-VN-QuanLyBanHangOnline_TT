using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Users;

namespace QuanLyBanHangOnline.Controllers
{
    [Authorize(Roles = "Staff,Admin")]  // Chỉ cần là Staff HOẶC Admin là vào được
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Staffs
        [HttpGet]
        [Authorize(Roles = "Admin")] // Chỉ Admin mới được xem toàn bộ danh sách
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaff()
        {
          if (_context.Staff == null)
          {
              return NotFound();
          }
            return await _context.Staff.Select(s => new StaffDto
            {
                IdStaff = s.IdStaff,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                Address = s.Address,
            }).ToListAsync();
        }

        // GET: api/Staffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetStaff(int id)
        {

            // 1. Lấy ID của người dùng hiện tại từ Claims trong Token
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // 2. Nếu là Staff, nhưng ID yêu cầu khác với ID của chính mình -> Chặn luôn
            if (currentUserRole == "Staff" && currentUserId != id.ToString())
            {
                return Forbid(); // Trả về 403 Forbidden
            }


            if (_context.Staff == null) return NotFound();
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)  return NotFound();
            var staffdto = new StaffDto {
                IdStaff = staff.IdStaff,
                FullName = staff.FullName,
                Email = staff.Email,
                Phone = staff.Phone,
                Address = staff.Address,
            };
            return staffdto;
        }

        // PUT: api/Staffs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, UpdateStaffDto staffdto)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Chỉ Admin hoặc chính Staff đó mới được sửa
            if (currentUserRole == "Staff" && currentUserId != id.ToString())
            {
                return Forbid();
            }



            var staff = await _context.Staff.FindAsync(id);
            if(staff == null) { return NotFound(); }
            staff.FullName = staffdto.FullName ?? staff.FullName;
            staff.Phone = staffdto.Phone ?? staff.Phone;
            staff.Address = staffdto.Address ?? staff.Address;

            if (!string.IsNullOrEmpty(staffdto.Password))
            {
                staff.Password = BCrypt.Net.BCrypt.HashPassword(staffdto.Password);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Staffs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostStaff(CreatStaffDto dto)
        {
            var staff = new Staff {
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? "Chua cap nhat",
                Phone = dto.Phone ?? "Chua cap nhat",
                Address = dto.Address ?? "Chua cap nhat",
            };
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Staffs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // // Chỉ Admin mới được xóa, Staff không có quyền này
        public async Task<IActionResult> DeleteStaff(int id)
        {
            if (_context.Staff == null)
            {
                return NotFound();
            }
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StaffExists(int id)
        {
            return (_context.Staff?.Any(e => e.IdStaff == id)).GetValueOrDefault();
        }
  
    }
}
