using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Products;
using QuanLyBanHangOnline.Helpers;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Controllers.Products
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService, IAppAuthorizationService authService) : base(authService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetProducts([FromQuery] PaginationParams @params)
        {
            var result = await _productService.GetAllAsync(@params);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> PostProduct([FromForm] ProductCreateDto dto)
        {
            // 1. Kiểm tra quyền hạn từ BaseController
            if (!await HasPermission("product_post")) return Forbid();

            // 2. Lấy IdAccount của người đang đăng nhập từ JWT Token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized("Không tìm thấy định danh người dùng.");

            int currentUserId = int.Parse(userIdClaim);

            try
            {
                // 3. Truyền thêm currentUserId vào Service để tự động tạo phiếu nhập (Import)
                var result = await _productService.CreateAsync(dto, currentUserId);
                return CreatedAtAction(nameof(GetProduct), new { id = result.IdSP }, result);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi nghiệp vụ (ví dụ: lỗi lưu file, lỗi DB)
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] ProductCreateDto dto)
        {
            if (!await HasPermission("product_edit")) return Forbid();
            var result = await _productService.UpdateAsync(id, dto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!await HasPermission("product_delete")) return Forbid(); 
            var result = await _productService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }

}
