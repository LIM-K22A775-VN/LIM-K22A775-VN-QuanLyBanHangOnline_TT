using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO;
using quanlybanhangonline.Models.DTOs;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "Staff,Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrder()
    {
        return Ok(await _orderService.GetAllOrdersAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(await _orderService.GetMyOrdersAsync(userId));
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> PostOrder(OrderRequestDto request)
    {
        if (request.Items == null || !request.Items.Any()) return BadRequest("Đơn hàng trống.");

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            var order = await _orderService.CreateOrderAsync(userId, request);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.IdDH }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/status")]
    [Authorize] // Bỏ Roles để cả User, Staff, Admin đều vào được
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] Enums.OrderStatus newStatus)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        // Truyền thêm userId và role vào Service để kiểm tra chính chủ
        var result = await _orderService.UpdateStatusAsync(id, newStatus, userId, role);

        if (!result) return BadRequest("Không thể cập nhật trạng thái đơn hàng (đơn đã giao hoặc không có quyền).");
        return Ok(new { message = "Cập nhật trạng thái thành công" });
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        string role = User.FindFirstValue(ClaimTypes.Role);

        var result = await _orderService.DeleteOrderAsync(id, userId, role);
        if (!result) return Forbid();

        return NoContent();
    }

    // Trong OrdersController.cs
    [HttpGet("{id}/details")]
    [Authorize] // Bất kỳ ai đã đăng nhập đều có thể gọi
    public async Task<IActionResult> GetDetails(int id)
    {
        // Lấy thông tin đơn hàng để kiểm tra quyền sở hữu
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

        // BẢO MẬT: Chỉ cho xem nếu là Admin/Staff HOẶC chính chủ đơn hàng đó
        if (currentUserRole != "Admin" && currentUserRole != "Staff" && order.IdUser != currentUserId)
        {
            return Forbid();
        }

        var details = await _orderService.GetDetailsByOrderIdAsync(id);
        return Ok(details);
    }

    [HttpPut("details/{detailId}")]
    [Authorize(Roles = "Staff,Admin")]
    public async Task<IActionResult> UpdateDetail(int detailId, OrderDetail detail)
    {
        var result = await _orderService.UpdateOrderDetailAsync(detailId, detail);
        if (!result) return BadRequest("Không thể cập nhật chi tiết đơn hàng.");
        return NoContent();
    }
}