using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Staffs;
using QuanLyBanHangOnline.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyBanHangOnline.Controllers.Staffs
{
    [Authorize(Roles = "Staff,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        // Inject IStaffService vào constructor
        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: api/Staffs
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<StaffDto>>> GetStaff([FromQuery] PaginationParams @params)
        {
            var staffs = await _staffService.GetAllAsync(@params);
            return Ok(staffs);
        }

        // GET: api/Staffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetStaff(int id)
        {
            // Kiểm tra quyền: Chỉ Admin hoặc chính Staff đó mới được xem
            if (!IsOwnerOrAdmin(id))
            {
                return Forbid();
            }

            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return Ok(staff);
        }

        // PUT: api/Staffs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStaff(int id, UpdateStaffDto staffdto)
        {
            // Kiểm tra quyền: Chỉ Admin hoặc chính Staff đó mới được sửa
            if (!IsOwnerOrAdmin(id))
            {
                return Forbid();
            }

            var result = await _staffService.UpdateAsync(id, staffdto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Staffs
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostStaff(CreatStaffDto dto)
        {
            try
            {
                await _staffService.CreateAsync(dto);
                return Ok(new { message = "Nhân viên đã được tạo thành công" });
            }
            catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Staffs/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Hàm phụ kiểm tra xem người dùng hiện tại có phải là chủ sở hữu tài khoản hoặc là Admin không.
        /// </summary>
        private bool IsOwnerOrAdmin(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            return currentUserRole == "Admin" || currentUserId == id.ToString();
        }
    }
}