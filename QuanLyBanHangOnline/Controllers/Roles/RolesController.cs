using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Role;
using QuanLyBanHangOnline.Models;

namespace QuanLyBanHangOnline.Controllers.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole()
        {
            if (_context.Role == null)
            {
                return NotFound();
            }
            return await _context.Role.ToListAsync();
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            if (_context.Role == null)
            {
                return NotFound();
            }
            var role = await _context.Role.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole([FromBody] RoleCreateUpdateDto dto)
        {
            var role = new Role
            {
                Title = dto.Title,
                Description = dto.Description,
                Permissions = "[]" // Mặc định mảng rỗng cho Role mới
            };

            _context.Role.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();
            _context.Role.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return (_context.Role?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [HttpPatch("update-permissions/{id}")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UpdatePermissionsDto dto)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null) return NotFound();

            // Chuyển mảng List thành chuỗi JSON để lưu vào database
            role.Permissions = Newtonsoft.Json.JsonConvert.SerializeObject(dto.Permissions);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật phân quyền thành công" });
        }
    }
}
