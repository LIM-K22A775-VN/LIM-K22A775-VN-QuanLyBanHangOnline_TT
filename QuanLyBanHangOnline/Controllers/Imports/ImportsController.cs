using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Import;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Controllers.Imports
{
    [Authorize] // Bắt buộc đăng nhập
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : BaseController
    {
        private readonly IImportService _importService;
        private readonly ApplicationDbContext _context; // Cần dùng để truy vấn GET hoặc chuyển vào Service

        public ImportsController(
            IImportService importService,
            IAppAuthorizationService authService,
            ApplicationDbContext context) : base(authService)
        {
            _importService = importService;
            _context = context;
        }

        // GET: api/Imports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportResponseDto>>> GetImports()
        {
            // Kiểm tra quyền xem danh sách
            if (!await HasPermission("import_view")) return Forbid();

            var result = await _importService.GetAllImportsAsync();
            return Ok(result);
        }

        // GET: api/Imports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImportResponseDto>> GetImport(int id)
        {
            if (!await HasPermission("import_view")) return Forbid();

            var result = await _importService.GetImportByIdAsync(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        // POST: api/Imports
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        public async Task<ActionResult<ImportResponseDto>> PostImport(ImportRequestDto dto)
        {
            // 1. Kiểm tra quyền hạn cụ thể cho việc nhập hàng
            if (!await HasPermission("import_post")) return Forbid();

            // 2. Lấy ID người dùng từ Token thông qua Claim đã nạp trong JwtUtils
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            try
            {
                var result = await _importService.CreateImportAsync(dto, int.Parse(userIdClaim));
                return CreatedAtAction(nameof(GetImport), new { id = result.IdImport }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private ImportResponseDto MapToResponseDto(Import import)
        {
            return new ImportResponseDto
            {
                IdImport = import.IdImport,
                ImportDate = import.ImportDate,
                IdAccount = import.IdAccount,
                Email = import.Account?.Email ?? "N/A", 
                TotalCost = import.TotalCost,
                Details = import.ImportDetails.Select(d => new ImportDetailResponseDto
                {
                    IdImportDetail = d.IdImportDetail,
                    IdSP = d.IdSP,
                    ProductName = d.Product?.Name ?? "N/A",
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };
        }
    }
}