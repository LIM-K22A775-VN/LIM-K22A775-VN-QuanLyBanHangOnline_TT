using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Orders;
using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;
using quanlybanhangonline.Models.DTOs; // Chứa OrderRequestDto
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Controllers.Orders
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrder()
        {
            var orders = await _context.Order
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponseDto
                {
                    IdDH = o.IdDH,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    CustomerName = o.User.FullName
                }).ToListAsync();

            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            var order = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderDetails) // Tên DbSet của bạn trong DbContext
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.IdDH == id);

            if (order == null) return NotFound();

            // Ánh xạ sang DTO
            var response = new OrderResponseDto
            {
                IdDH = order.IdDH,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(), // "ChoXacNhan", "DaXacNhan"...
                CustomerName = order.User?.FullName,
                Items = order.OrderDetails.Select(od => new OrderDetailResponseDto
                {
                    IdSP = od.IdSP,
                    ProductName = od.Product?.Name ?? "Sản phẩm không tồn tại",
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Ok(response);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.IdDH)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // POST: api/Orders
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> PostOrder(OrderRequestDto request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest("Đơn hàng phải có ít nhất một sản phẩm.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Không thể xác định danh tính người dùng.");
            }

            // 1. Khởi tạo đối tượng Order và danh sách chi tiết cùng lúc
            var newOrder = new Order
            {
                IdUser = currentUserId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.ChoXacNhan,
                TotalPrice = 0,
                OrderDetails = new List<OrderDetail>() // Khởi tạo list trống
            };

            // 2. Duyệt danh sách sản phẩm
            foreach (var item in request.Items)
            {
                var product = await _context.Product.FindAsync(item.IdSP);
                if (product == null) return BadRequest($"Sản phẩm ID {item.IdSP} không tồn tại.");

                // 3. Tạo detail và add trực tiếp vào newOrder.OrderDetails
                var detail = new OrderDetail
                {
                    IdSP = item.IdSP,
                    Quantity = item.Quantity,
                    Price = product.Price,
                    // Không cần gán Order = newOrder vì đã add vào list của nó
                };

                newOrder.TotalPrice += (detail.Price * detail.Quantity);
                newOrder.OrderDetails.Add(detail);
            }

            // 4. Chỉ cần Add đối tượng Cha (Order), EF sẽ tự lưu các đối tượng Con (OrderDetails)
            _context.Order.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.IdDH }, new { message = "Đặt hàng thành công", orderId = newOrder.IdDH });
        }
        // DELETE: api/Orders/5
        [Authorize] // Chỉ cần đăng nhập là vào được hàm này
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null) return NotFound();

            // 1. Lấy thông tin từ Token (Lưu ý: Value luôn là string)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Chuyển đổi string sang int an toàn
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("Không thể xác định danh tính người dùng.");
            }

            // 2. Kiểm tra logic bảo mật
            // Admin thì được xóa hết, còn User thường chỉ được xóa đơn của chính mình
            if (currentUserRole != "Admin" && currentUserRole != "Staff" && order.IdUser != currentUserId)
            {
                return Forbid(); // Trả về 403 Forbidden
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.IdDH == id)).GetValueOrDefault();
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = int.Parse(userIdClaim);

            var orders = await _context.Order
                .Where(o => o.IdUser == currentUserId)
                .Include(o => o.OrderDetails) // Phải Include chi tiết
                    .ThenInclude(od => od.Product) // Và Include sản phẩm để lấy tên
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderResponseDto
                {
                    IdDH = o.IdDH,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    CustomerName = o.User.FullName,
                    // THÊM ĐOẠN NÀY ĐỂ HẾT NULL:
                    Items = o.OrderDetails.Select(od => new OrderDetailResponseDto
                    {
                        IdSP = od.IdSP,
                        ProductName = od.Product.Name,
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList()
                }).ToListAsync();

            return Ok(orders);
        }



        [Authorize(Roles = "Staff,Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus newStatus)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null) return NotFound();

            // Chặn không cho phép cập nhật nếu đơn đã giao hoặc đã hủy
            if (order.Status == OrderStatus.DaNhanHang || order.Status == OrderStatus.DaHuy)
            {
                return BadRequest("Không thể thay đổi trạng thái của đơn hàng đã hoàn thành hoặc đã hủy.");
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", newStatus = order.Status.ToString() });
        }
    }
}
