using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Staffs;
using QuanLyBanHangOnline.Helpers;
using QuanLyBanHangOnline.Services.Interfaces;
using System.Security.Claims;

namespace QuanLyBanHangOnline.Controllers.Staffs
{
    [Authorize(Roles = "Staff,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    // Kế thừa BaseController để dùng HasPermission check DB trực tiếp
    public class StaffsController : BaseController
    {
        private readonly IStaffService _staffService;

        // Inject IStaffService vào constructor
        public StaffsController(IStaffService staffService, IAppAuthorizationService authService) : base(authService)
        {
            _staffService = staffService;
        }

        // GET: api/Staffs
        [HttpGet]
        public async Task<ActionResult<PagedResult<StaffDto>>> GetStaff([FromQuery] PaginationParams @params)
        {
            // Kiểm tra: Admin được vào, hoặc Staff có quyền "staff_view"
            if (!await HasPermission("staff_view"))
            {
                return Forbid();
            }

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
            // Logic: 
            // 1. Nếu là Admin hoặc chính chủ -> Cho phép sửa (thông tin cá nhân)
            // 2. Nếu là Staff khác -> Phải có quyền "staff_edit" mới được sửa người khác
            bool isOwnerOrAdmin = IsOwnerOrAdmin(id);
            bool hasEditPermission = await HasPermission("staff_edit");

            if (!isOwnerOrAdmin && !hasEditPermission)
            {
                return Forbid();
            }

            // Bảo mật bổ sung: Chỉ Admin mới được phép thay đổi RoleId của người khác
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            if (currentUserRole != "Admin")
            {
                staffdto.RoleId = null; // Chặn nhân viên tự nâng quyền hoặc đổi quyền người khác
                staffdto.Salary = null; // Không được tự tăng lương
            }
            // Nếu staffdto.Salary có giá trị, nó sẽ lấy giá trị đó.
            // Nếu staffdto.Salary là null, nó sẽ giữ nguyên giá trị cũ (staff.Salary) đang có trong DB.

            var result = await _staffService.UpdateAsync(id, staffdto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Staffs
        [HttpPost]
        public async Task<IActionResult> PostStaff(CreatStaffDto dto)
        {
            // Dùng mã quyền "staff_create" để kiểm tra từ Ma trận
            if (!await HasPermission("staff_post"))
            {
                return Forbid();
            }

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
        public async Task<IActionResult> DeleteStaff(int id)
        {
            // Chỉ Admin hoặc người có quyền xóa nhân viên mới được thực hiện
            if (!await HasPermission("staff_delete"))
            {
                return Forbid();
            }

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

            // Nếu là Admin thì luôn có quyền
            if (currentUserRole == "Admin") return true;

            // Nếu không phải Admin, thì ID người đăng nhập phải khớp với ID tài khoản đang thao tác
            return currentUserId == id.ToString();
        }
    }
}