using QuanLyBanHangOnline.DTO;
using quanlybanhangonline.Models.DTOs;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId);
    Task<OrderResponseDto> CreateOrderAsync(int userId, OrderRequestDto request);
    Task<bool> UpdateStatusAsync(int id, Enums.OrderStatus newStatus, int userId, string userRole);
    Task<bool> DeleteOrderAsync(int id, int userId, string userRole);

    // Các hàm thao tác với Chi tiết đơn hàng (OrderDetail)
    Task<IEnumerable<OrderDetailResponseDto>> GetDetailsByOrderIdAsync(int orderId);
    Task<bool> UpdateOrderDetailAsync(int detailId, OrderDetail detail);
    Task<bool> DeleteOrderDetailAsync(int detailId);
}