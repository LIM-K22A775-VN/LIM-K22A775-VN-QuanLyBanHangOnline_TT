using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.DTO.Cart;
using QuanLyBanHangOnline.DTO.Cart;
using quanlybanhangonline.Model;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetAllCartsForAdminAsync()
        {
            return await _context.Cart
                .Include(c => c.User)
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .Select(c => new {
                    c.IdCart,
                    c.User.IdAccount,
                    UserEmail = c.User.Email,
                    c.Status,
                    ItemCount = c.CartDetails.Count,
                    TotalAmount = c.CartDetails.Sum(cd => cd.Quantity * (cd.Product != null ? cd.Product.Price : 0)),
                    Details = c.CartDetails.Select(cd => new {
                        cd.IdSP,
                        ProductName = cd.Product.Name,
                        cd.Quantity,
                        Price = cd.Product.Price
                    })
                }).ToListAsync();
        }

        public async Task<CartResultDto> GetMyCartAsync(int userId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.IdUser == userId);

            if (cart == null) return new CartResultDto { Details = new List<CartDetailResultDto>() };

            return new CartResultDto
            {
                IdCart = cart.IdCart,
                Status = cart.Status,
                Details = cart.CartDetails.Select(cd => new CartDetailResultDto
                {
                    IdCartDetail = cd.IdCartDetail,
                    IdSP = cd.IdSP,
                    ProductName = cd.Product?.Name ?? "N/A",
                    Image = cd.Product?.Image,
                    Price = cd.Product?.Price ?? 0,
                    Quantity = cd.Quantity
                }).ToList()
            };
        }

        public async Task<string> AddToCartAsync(AddToCartDto dto, int userId)
        {
            // 1. Tìm giỏ hàng của người dùng
            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.IdUser == userId);

            if (cart == null)
            {
                // 2. Nếu CHƯA có giỏ hàng: Tạo mới Giỏ hàng và gán luôn dòng Chi tiết đầu tiên
                // Entity Framework sẽ tự động hiểu quan hệ cha-con và gán IdCart cho Detail
                cart = new Cart
                {
                    IdUser = userId,
                    Status = 1, // Trạng thái có hàng
                    CartDetails = new List<CartDetail>
            {
                new CartDetail { IdSP = dto.IdSP, Quantity = 1 }
            }
                };
                _context.Cart.Add(cart);
            }
            else
            {
                // 3. Nếu ĐÃ có giỏ hàng: Kiểm tra xem sản phẩm này đã có trong giỏ chưa
                var cartDetail = await _context.CartDetail
                    .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == dto.IdSP);

                if (cartDetail == null)
                {
                    // Nếu chưa có thì mới thêm dòng mới
                    var newDetail = new CartDetail
                    {
                        IdCart = cart.IdCart,
                        IdSP = dto.IdSP,
                        Quantity = 1
                    };
                    _context.CartDetail.Add(newDetail);
                }
                // Theo yêu cầu của bạn: Nếu đã có thì không cần làm gì thêm (không tăng số lượng)

                cart.Status = 1; // Cập nhật trạng thái giỏ hàng
            }

            // 4. Chỉ gọi SaveChanges một lần duy nhất để tối ưu hiệu năng
            await _context.SaveChangesAsync();
            return "Đã thêm sản phẩm vào giỏ hàng thành công!";
        }
        public async Task<string> UpdateQuantityAsync(UpdateCartDto dto, int userId)
        {
            var product = await _context.Product.FindAsync(dto.IdSP);
            if (product == null) throw new Exception("Sản phẩm không tồn tại.");
            if (dto.Quantity > product.StockQuantity) throw new Exception($"Tồn kho không đủ ({product.StockQuantity}).");
            if (dto.Quantity <= 0) throw new Exception("Số lượng phải lớn hơn 0.");

            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.IdUser == userId);
            var cartDetail = await _context.CartDetail
                .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == dto.IdSP);

            if (cartDetail == null) throw new Exception("Sản phẩm không có trong giỏ hàng.");

            cartDetail.Quantity = dto.Quantity;
            await _context.SaveChangesAsync();
            return "Cập nhật số lượng thành công!";
        }

        public async Task<string> RemoveProductFromCartAsync(int idSP, int userId)
        {
            var cart = await _context.Cart.FirstOrDefaultAsync(c => c.IdUser == userId);
            var cartDetail = await _context.CartDetail
                .FirstOrDefaultAsync(cd => cd.IdCart == cart.IdCart && cd.IdSP == idSP);

            if (cartDetail == null) throw new Exception("Sản phẩm không có trong giỏ hàng.");

            _context.CartDetail.Remove(cartDetail);
            await _context.SaveChangesAsync();

            if (!await _context.CartDetail.AnyAsync(cd => cd.IdCart == cart.IdCart))
            {
                cart.Status = 0;
                await _context.SaveChangesAsync();
            }
            return "Đã xóa sản phẩm khỏi giỏ hàng!";
        }
    }
}
