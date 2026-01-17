using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.Services.Interfaces;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Controllers.Admins
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmin()
        {
            var admins = await _adminService.GetAllAsync();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            var result = await _adminService.UpdateAsync(id, admin);
            if (!result) return BadRequest("Cập nhật thất bại hoặc ID không khớp");
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            await _adminService.CreateAsync(admin);
            return CreatedAtAction("GetAdmin", new { id = admin.IdAdmin }, admin);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var result = await _adminService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}