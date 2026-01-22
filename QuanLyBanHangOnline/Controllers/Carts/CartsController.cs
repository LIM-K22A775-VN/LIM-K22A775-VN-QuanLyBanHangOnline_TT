using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.DTO.Cart;
using quanlybanhangonline.Model;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.DTO.Cart;
using QuanLyBanHangOnline.Services.Interfaces;
namespace QuanLyBanHangOnline.Controllers.Carts
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : BaseController
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService, IAppAuthorizationService authService)
            : base(authService)
        {
            _cartService = cartService;
        }

        // GET: api/Carts - Dành cho Admin/Staff xem toàn bộ giỏ hàng
        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            // Kiểm tra quyền quản lý giỏ hàng (slug: cart_view)
            if (!await HasPermission("cart_view")) return Forbid();

            return Ok(await _cartService.GetAllCartsForAdminAsync());
        }

        // GET: api/Carts/my-cart - Người dùng xem giỏ của chính mình
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            // Không cần slug vì đây là quyền cơ bản của mọi User đã login
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _cartService.GetMyCartAsync(userId));
        }

        [HttpPost]
        public async Task<IActionResult> PostCart(AddToCartDto dto)
        {
            // Mọi User đều có quyền thêm vào giỏ 
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(new { message = await _cartService.AddToCartAsync(dto, userId) });
            }
            catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateQuantity(UpdateCartDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(new { message = await _cartService.UpdateQuantityAsync(dto, userId) });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("remove-product/{idSP}")]
        public async Task<IActionResult> RemoveProduct(int idSP)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Ok(new { message = await _cartService.RemoveProductFromCartAsync(idSP, userId) });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
    }
}