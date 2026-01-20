using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Role;
using QuanLyBanHangOnline.DTO.Roles;
using QuanLyBanHangOnline.Helpers;
using QuanLyBanHangOnline.Services.Interfaces;

        namespace QuanLyBanHangOnline.Controllers.Roles
        {
            [Authorize] // Bắt buộc đăng nhập
            [Route("api/[controller]")]
            [ApiController]
            public class RolesController :  BaseController
            {
                private readonly ApplicationDbContext _context;

                public RolesController(ApplicationDbContext context, IAppAuthorizationService authService) : base(authService)
        {
                    _context = context;
                }

                // GET: api/Roles
                [HttpGet]
                public async Task<ActionResult<IEnumerable<Role>>> GetRole()
                {
                    // Chỉ ai có quyền xem nhóm quyền hoặc Admin mới được vào
                    if (!await HasPermission("role_view"))
                    {
                        return Forbid();
                    }
                    return await _context.Role.ToListAsync();
                }

                // GET: api/Roles/5
                [HttpGet("{id}")]
                public async Task<ActionResult<Role>> GetRole(int id)
                {
                    if (!await HasPermission("role_view")) return Forbid();

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
                public async Task<IActionResult> PutRole(int id, RoleUpdateDto dto)
                {
                    // Bổ sung check quyền sửa thông tin Role
                    if (!await HasPermission("role_edit")) return Forbid();
                    var role = await _context.Role.FindAsync(id);
                    if (role == null) return NotFound();
                    // Chỉ cập nhật 2 trường này
                    role.Title = dto.Title;
                    role.Description = dto.Description;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RoleExists(id)) return NotFound();
                        else throw;
                    }

                    return NoContent();
                }

                // POST: api/Roles
                // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
                [HttpPost]
                public async Task<ActionResult<Role>> PostRole([FromBody] RoleCreateUpdateDto dto)
                {
                    if (!await HasPermission("role_post")) return Forbid();

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
                    if (!await HasPermission("role_delete")) return Forbid();

                    var role = await _context.Role.FindAsync(id);
                    if (role == null) return NotFound();

                    // KIỂM TRA: Nếu có Staff đang dùng Role này thì không cho xóa
                    var hasStaff = await _context.Staff.AnyAsync(s => s.RoleId == id);
                    if (hasStaff)
                    {
                        return BadRequest(new { message = "Không thể xóa nhóm quyền này vì đang có nhân viên sử dụng." });
                    }

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
                    // Cần một mã quyền riêng biệt và cao cấp để sửa ma trận quyền
                    if (!await HasPermission("role_permission"))
                    {
                        return Forbid();
                    }

                    var role = await _context.Role.FindAsync(id);
                    if (role == null) return NotFound();

                    // Chuyển mảng List thành chuỗi JSON để lưu vào database
                    role.Permissions = Newtonsoft.Json.JsonConvert.SerializeObject(dto.Permissions);

                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Cập nhật phân quyền thành công" });
                }
            }
        }
