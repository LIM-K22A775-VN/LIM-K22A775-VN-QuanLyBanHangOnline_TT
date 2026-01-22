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

namespace QuanLyBanHangOnline.Controllers.Carts
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        // GET: api/Carts
        // Dành cho Admin/Staff quản lý toàn bộ giỏ hàng hệ thống
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCart()
        {
            // Lấy tất cả giỏ hàng kèm thông tin User và các chi tiết món ăn
            var allCarts = await _context.Cart
                .Include(c => c.User)
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .Select(c => new {
                    c.IdCart,
                    c.User.IdUser,
                    UserEmail = c.User.Email, // Để Admin biết giỏ hàng của ai
                    c.Status,
                    ItemCount = c.CartDetails.Count, // Số lượng loại sản phẩm trong giỏ
                    TotalAmount = c.CartDetails.Sum(cd => cd.Quantity * (cd.Product != null ? cd.Product.Price : 0)),
                    // Có thể trả về chi tiết nếu Admin muốn xem sâu hơn
                    Details = c.CartDetails.Select(cd => new {
                        cd.IdSP,
                        ProductName = cd.Product.Name,
                        cd.Quantity,
                        Price = cd.Product.Price
                    })
                })
                .ToListAsync();

            return Ok(allCarts);
        }
        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            if (_context.Cart == null)
            {
                return NotFound();
            }
            var cart = await _context.Cart.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }
        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart(AddToCartDto dto)
        {
            // 1. Lấy IdUser an toàn từ Token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            // 2. Tìm Giỏ hàng của User, nếu chưa có thì tạo mới
            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.IdUser == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    IdUser = userId,
                    Status = 1 // 1: Có hàng
                };
                _context.Cart.Add(cart);
                // Save để lấy IdCart mới sinh ra
                await _context.SaveChangesAsync();
            }

            // 3. Kiểm tra sản phẩm này đã có trong giỏ chưa
            var cartDetail = await _context.CartDetail
                .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == dto.IdSP);

            if (cartDetail != null)
            {
                // Nếu đã có: Tăng số lượng thêm 1
                cartDetail.Quantity += 1;
                _context.Entry(cartDetail).State = EntityState.Modified;
            }
            else
            {
                // Nếu chưa có: Thêm mới với số lượng mặc định là 1
                var newDetail = new CartDetail
                {
                    IdCart = cart.IdCart,
                    IdSP = dto.IdSP,
                    Quantity = 1
                };
                _context.CartDetail.Add(newDetail);
            }

            // 4. Cập nhật trạng thái giỏ hàng là "Có hàng"
            cart.Status = 1;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã thêm sản phẩm vào giỏ hàng thành công!" });
        }
        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            if (_context.Cart == null)
            {
                return NotFound();
            }
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int id)
        {
            return (_context.Cart?.Any(e => e.IdCart == id)).GetValueOrDefault();
        }


        // GET: api/Carts/my-cart
        [HttpGet("my-cart")]
        [Authorize] // Chỉ sinh viên đã đăng nhập mới xem được giỏ của mình
        public async Task<ActionResult<CartResultDto>> GetMyCart()
        {
            // 1. Lấy IdUser từ Token (NameIdentifier)
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            // 2. Tìm giỏ hàng kèm theo Chi tiết và Thông tin sản phẩm
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.IdUser == userId);

            // 3. Nếu chưa có giỏ hàng, trả về DTO rỗng để Angular không bị lỗi
            if (cart == null)
            {
                return Ok(new CartResultDto
                {
                    Details = new List<CartDetailResultDto>()
                });
            }

            // 4. Ánh xạ (Map) sang DTO để trả về dữ liệu "sạch"
            var result = new CartResultDto
            {
                IdCart = cart.IdCart,
                Status = cart.Status,
                Details = cart.CartDetails.Select(cd => new CartDetailResultDto
                {
                    IdCartDetail = cd.IdCartDetail,
                    IdSP = cd.IdSP,
                    ProductName = cd.Product?.Name ?? "Sản phẩm không tồn tại",
                    Image = cd.Product?.Image,
                    Price = cd.Product?.Price ?? 0,
                    Quantity = cd.Quantity
                }).ToList()
            };

            return Ok(result);
        }

        // DELETE: api/Carts/remove-item/5
        // DELETE: api/Carts/remove-product/10
        [HttpDelete("remove-product/{idSP}")]
        [Authorize]
        public async Task<IActionResult> RemoveProductFromCart(int idSP)
        {
            // 1. Lấy IdUser từ Token để đảm bảo xóa đúng giỏ hàng của mình
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            // 2. Tìm giỏ hàng của User này
            var cart = await _context.Cart
                .FirstOrDefaultAsync(c => c.IdUser == userId);

            if (cart == null)
            {
                return NotFound(new { message = "Bạn chưa có giỏ hàng." });
            }

            // 3. Tìm dòng Detail có IdCart và IdSP khớp với yêu cầu
            var cartDetail = await _context.CartDetail
                .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == idSP);

            if (cartDetail == null)
            {
                return NotFound(new { message = "Sản phẩm không có trong giỏ hàng của bạn." });
            }

            // 4. Thực hiện xóa dòng chi tiết đó
            _context.CartDetail.Remove(cartDetail);

            // 5. Kiểm tra nếu xóa xong mà giỏ hàng hết sạch đồ thì cập nhật Status = 0
            await _context.SaveChangesAsync();

            var anyItemsLeft = await _context.CartDetail.AnyAsync(cd => cd.IdCart == cart.IdCart);
            if (!anyItemsLeft)
            {
                cart.Status = 0;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng thành công!" });
        }

        // PUT: api/Carts/update-quantity
        // PUT: api/Carts/update-quantity
        [HttpPut("update-quantity")]
        [Authorize]
        public async Task<IActionResult> UpdateQuantity(UpdateCartDto dto)
        {
            // 1. Lấy IdUser từ Token
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            // 2. Tìm sản phẩm trong kho để kiểm tra số lượng tồn
            var product = await _context.Product.FindAsync(dto.IdSP);
            if (product == null) return NotFound(new { message = "Sản phẩm không tồn tại." });

            // 3. KIỂM TRA TỒN KHO: Số lượng yêu cầu không được vượt quá số lượng đang có
            if (dto.Quantity > product.StockQuantity)
            {
                return BadRequest(new
                {
                    message = $"Số lượng yêu cầu ({dto.Quantity}) vượt quá tồn kho hiện có ({product.StockQuantity})."
                });
            }

            if (dto.Quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng phải lớn hơn 0." });
            }

            // 4. Tìm giỏ hàng và dòng chi tiết tương ứng
            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.IdUser == userId);
            if (cart == null) return NotFound(new { message = "Không tìm thấy giỏ hàng." });

            var cartDetail = await _context.CartDetail
                .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == dto.IdSP);

            if (cartDetail == null)
            {
                return NotFound(new { message = "Sản phẩm không có trong giỏ hàng của bạn." });
            }

            // 5. Cập nhật số lượng
            cartDetail.Quantity = dto.Quantity;
            _context.Entry(cartDetail).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã cập nhật số lượng thành công!" });
        }
    }
}
