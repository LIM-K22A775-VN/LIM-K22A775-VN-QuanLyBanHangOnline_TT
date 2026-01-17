using QuanLyBanHangOnline.DTO;
using quanlybanhangonline.Models.DTOs;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;
using QuanLyBanHangOnline.DTO.Generic;

public interface IOrderService
{
    Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync(PaginationParams @params);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<PagedResult<OrderResponseDto>> GetMyOrdersAsync(int userId, PaginationParams @params);
    Task<OrderResponseDto> CreateOrderAsync(int userId, OrderRequestDto request);
    Task<bool> UpdateStatusAsync(int id, Enums.OrderStatus newStatus, int userId, string userRole);
    Task<bool> DeleteOrderAsync(int id, int userId, string userRole);

    // Các hàm thao tác với Chi tiết đơn hàng (OrderDetail)
    Task<IEnumerable<OrderDetailResponseDto>> GetDetailsByOrderIdAsync(int orderId);
    Task<bool> UpdateOrderDetailAsync(int detailId, OrderDetail detail);
    Task<bool> DeleteOrderDetailAsync(int detailId);
}