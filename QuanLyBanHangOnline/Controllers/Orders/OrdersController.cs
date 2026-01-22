using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO;
using quanlybanhangonline.Models.DTOs;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;
using System.Security.Claims;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : BaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService, IAppAuthorizationService authService) : base(authService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetOrder([FromQuery] PaginationParams @params)
    {
        if (!await HasPermission("order_view")) return Forbid();
        return Ok(await _orderService.GetAllOrdersAsync(@params));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();
        // Logic: Admin/Staff có quyền 'order_view' HOẶC Khách hàng là chủ đơn hàng
        bool isOwner = order.IdUser == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        bool hasViewPermission = await HasPermission("order_view");

        if (!isOwner && !hasViewPermission) return Forbid();
        return Ok(order);
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetMyOrders([FromQuery] PaginationParams @params)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(await _orderService.GetMyOrdersAsync(userId, @params));
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

        // Kiểm tra quyền: Nhân viên cần 'order_edit' để đổi trạng thái
        // Lưu ý: User (Khách) có thể hủy đơn nếu Service cho phép (chưa giao hàng)
        if (role != "User" && !await HasPermission("order_edit"))
        {
            return Forbid();
        }
        try
        {
            // Truyền thêm userId và role vào Service để kiểm tra chính chủ
            var result = await _orderService.UpdateStatusAsync(id, newStatus, userId, role);
            if (!result) return BadRequest("Không thể cập nhật trạng thái đơn hàng (đơn đã giao hoặc không có quyền).");
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {

        // Chỉ Admin hoặc Staff có quyền 'order_delete' mới được xóa đơn (thường rất hạn chế)
        if (!await HasPermission("order_delete")) return Forbid();

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

        // Check quyền xem chi tiết tương tự xem đơn hàng
        if (order.IdUser != currentUserId && !await HasPermission("order_view"))
        {
            return Forbid();
        }

        var details = await _orderService.GetDetailsByOrderIdAsync(id);
        return Ok(details);
    }

    [HttpPut("details/{detailId}")]
    public async Task<IActionResult> UpdateDetail(int detailId, OrderDetail detail)
    {
        if (!await HasPermission("order_edit")) return Forbid();
        var result = await _orderService.UpdateOrderDetailAsync(detailId, detail);
        if (!result) return BadRequest("Không thể cập nhật chi tiết đơn hàng.");
        return NoContent();
    }
}