    using Microsoft.EntityFrameworkCore;
    using QuanLyBanHangOnline.DTO;
    using QuanLyBanHangOnline.Services.Interfaces;
    using quanlybanhangonline.Models;
    using quanlybanhangonline.Models.DTOs;
    using QuanLyBanHangOnline.Constants;
    using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;
    using QuanLyBanHangOnline.DTO.Generic;
    using QuanLyBanHangOnline.Helpers;

    namespace QuanLyBanHangOnline.Services.Implementations
    {
        public class OrderService : IOrderService
        {
            private readonly ApplicationDbContext _context;

            public OrderService(ApplicationDbContext context)
            {
                _context = context;
            }

            // 1. Cho Admin xem toàn bộ
            public async Task<PagedResult<OrderResponseDto>> GetAllOrdersAsync(PaginationParams @params)
            {
                var query = _context.Order
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => MapToResponseDto(o));

                return await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);
            }

            public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
            {
                var order = await _context.Order
                    .Include(o => o.User)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.IdDH == id);

                if (order == null) return null;

                return MapToResponseDto(order);
            }

            // 2. Cho Khách hàng xem đơn của họ
            public async Task<PagedResult<OrderResponseDto>> GetMyOrdersAsync(int userId, PaginationParams @params)
            {
                var query = _context.Order
                    .Where(o => o.IdUser == userId)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => MapToResponseDto(o));

                return await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);
            }
            public async Task<OrderResponseDto> CreateOrderAsync(int userId, OrderRequestDto request)
            {
                // Sử dụng Transaction để đảm bảo nếu trừ kho lỗi thì đơn hàng không được tạo
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var newOrder = new Order
                    {
                        IdUser = userId,
                        OrderDate = DateTime.Now,
                        Status = Enums.OrderStatus.ChoXacNhan,
                        TotalPrice = 0,
                        // --- CẬP NHẬT: Gán thông tin giao hàng từ DTO vào Model ---
                        ReceiverName = request.ReceiverName,
                        ReceiverPhone = request.ReceiverPhone,
                        ShippingAddress = request.ShippingAddress,
                        OrderNotes = request.OrderNotes,
                        OrderDetails = new List<OrderDetail>()
                    };

                    foreach (var item in request.Items)
                    {
                        var product = await _context.Product.FindAsync(item.IdSP);
                        if (product == null) throw new Exception($"Sản phẩm {item.IdSP} không tồn tại.");

                        // KIỂM TRA TỒN KHO
                        if (product.StockQuantity < item.Quantity)
                            throw new Exception($"Sản phẩm '{product.Name}' không đủ số lượng trong kho (Còn lại: {product.StockQuantity}).");

                        // TRỪ KHO
                        product.StockQuantity -= item.Quantity;

                        var detail = new OrderDetail
                        {
                            IdSP = item.IdSP,
                            Quantity = item.Quantity,
                            Price = product.Price
                        };

                        newOrder.TotalPrice += (detail.Price * detail.Quantity);
                        newOrder.OrderDetails.Add(detail);
                    }

                    _context.Order.Add(newOrder);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync(); // Hoàn tất giao dịch

                    return MapToResponseDto(newOrder);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(); // Nếu lỗi thì trả lại dữ liệu ban đầu
                    throw;
                }
            }

            public async Task<bool> UpdateStatusAsync(int id, Enums.OrderStatus newStatus, int userId, string userRole)
            {
                var order = await _context.Order
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.IdDH == id);

                if (order == null) return false;

                // 1. BẢO MẬT: Kiểm tra quyền sở hữu đơn hàng
                if (userRole != "Admin" && userRole != "Staff" && order.IdUser != userId)
                {
                    return false;
                }

                // 2. PHÂN QUYỀN LOGIC TRẠNG THÁI
                if (userRole != "Admin" && userRole != "Staff")
                {
                    // NẾU LÀ USER THƯỜNG:
                    if(newStatus != Enums.OrderStatus.DaHuy && newStatus != Enums.OrderStatus.ChoXacNhan) return false;
                }
                else
                {
                    // NẾU LÀ STAFF/ADMIN:
                    // Chặn không cho đổi trạng thái nếu đơn đã kết thúc Đã nhận
                    if (order.Status == Enums.OrderStatus.DaNhanHang)
                    {
                        return false;
                    }
                }

                // 3. XỬ LÝ HOÀN KHO KHI HỦY ĐƠN
                if (newStatus == Enums.OrderStatus.DaHuy)
                {
                    // Hoàn kho (vì User chỉ hủy được khi status = 0, Staff có thể hủy khi status = 0 hoặc 1)
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = await _context.Product.FindAsync(detail.IdSP);
                        if (product != null)
                        {
                            product.StockQuantity += detail.Quantity;
                        }
                    }
                }

                // 4. CẬP NHẬT
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteOrderAsync(int id, int userId, string userRole)
            {
                var order = await _context.Order
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.IdDH == id);

                if (order == null) return false;

                // Kiểm tra quyền sở hữu
                if (userRole != "Admin" && userRole != "Staff" && order.IdUser != userId)
                    return false;

                // CHỈ cho phép hủy (xóa) khi đơn đang chờ xác nhận
                if (order.Status != Enums.OrderStatus.ChoXacNhan)
                    return false;

                // Hoàn kho trước khi xóa (hoặc chuyển trạng thái)
                foreach (var detail in order.OrderDetails)
                {
                    var product = await _context.Product.FindAsync(detail.IdSP);
                    if (product != null)
                    {
                        product.StockQuantity += detail.Quantity;
                    }
                }

                _context.Order.Remove(order); // Hoặc order.Status = Enums.OrderStatus.DaHuy;
                await _context.SaveChangesAsync();
                return true;
            }

            // Hàm phụ dùng chung để ánh xạ dữ liệu
            private static OrderResponseDto MapToResponseDto(Order order)
            {
                return new OrderResponseDto
                {
                    IdDH = order.IdDH,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status.ToString(),
                    IdUser = order.IdUser,

                    // --- BỔ SUNG: Trả về thông tin giao hàng ---
                    ReceiverName = order.ReceiverName,
                    ReceiverPhone = order.ReceiverPhone,
                    ShippingAddress = order.ShippingAddress,
                    OrderNotes = order.OrderNotes ?? "Không có ghi chú",

                    Items = order.OrderDetails?.Select(od => new OrderDetailResponseDto
                    {
                        IdSP = od.IdSP,
                        ProductName = od.Product?.Name ?? "Sản phẩm không xác định",
                        Quantity = od.Quantity,
                        Price = od.Price
                    }).ToList() ?? new List<OrderDetailResponseDto>()
                };
            }
            public async Task<bool> UpdateOrderDetailAsync(int detailId, OrderDetail detail)
            {
                var existingDetail = await _context.OrderDetail
                    .Include(od => od.Product) // Cần product để trừ/cộng kho
                    .FirstOrDefaultAsync(od => od.IdOrderDetail == detailId);

                if (existingDetail == null) return false;

                var order = await _context.Order.Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.IdDH == existingDetail.IdDH);

                // Chỉ cho sửa nếu đơn đang chờ xác nhận
                if (order == null || order.Status != Enums.OrderStatus.ChoXacNhan) return false;

                // Tính chênh lệch: Số lượng mới - Số lượng cũ
                int diff = detail.Quantity - existingDetail.Quantity;

                // Nếu tăng số lượng, kiểm tra kho có đủ không
                if (diff > 0 && existingDetail.Product.StockQuantity < diff) return false;

                // Cập nhật kho
                existingDetail.Product.StockQuantity -= diff;

                // Cập nhật thông tin chi tiết
                existingDetail.Quantity = detail.Quantity;
                existingDetail.Price = detail.Price;

                // Tính lại tổng tiền cho đơn hàng
                order.TotalPrice = order.OrderDetails.Sum(d => d.Price * d.Quantity);

                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteOrderDetailAsync(int detailId)
            {
                // 1. Tìm món hàng kèm theo Product để hoàn kho
                var detail = await _context.OrderDetail
                    .Include(od => od.Product)
                    .FirstOrDefaultAsync(od => od.IdOrderDetail == detailId);

                if (detail == null) return false;

                // 2. Tìm đơn hàng cha
                var order = await _context.Order
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.IdDH == detail.IdDH);

                // 3. Chỉ cho xóa nếu đơn hàng đang "Chờ xác nhận"
                if (order == null || order.Status != Enums.OrderStatus.ChoXacNhan) return false;

                // 4. HOÀN KHO: Cộng lại số lượng vào Stock trước khi xóa bản ghi
                if (detail.Product != null)
                {
                    detail.Product.StockQuantity += detail.Quantity;
                }

                // 5. Xóa món hàng
                _context.OrderDetail.Remove(detail);

                // 6. Tính lại tổng tiền cho đơn hàng từ các món còn lại
                order.TotalPrice = order.OrderDetails
                    .Where(d => d.IdOrderDetail != detailId)
                    .Sum(d => d.Price * d.Quantity);

                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<IEnumerable<OrderDetailResponseDto>> GetDetailsByOrderIdAsync (int orderId)
            {
                return await _context.OrderDetail
                    .Where(od => od.IdDH == orderId)
                    .Include(od => od.Product) // Để lấy được ProductName
                    .Select(od => new OrderDetailResponseDto
                    {
                        IdOrderDetail = od.IdOrderDetail,
                        IdSP = od.IdSP,
                        ProductName = od.Product.Name, // Ánh xạ tên sản phẩm
                        Quantity = od.Quantity,
                        Price = od.Price
                        // SubTotal tự động tính theo công thức bạn đã viết trong DTO
                    })
                    .ToListAsync();
            }

        }
    }