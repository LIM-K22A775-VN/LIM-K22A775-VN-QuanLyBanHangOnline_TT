using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Products;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponseDto>> GetAllAsync(PaginationParams @params);

        Task<ProductResponseDto?> GetByIdAsync(int id);

        // CẬP NHẬT: Thêm tham số currentUserId để ghi nhận người nhập hàng vào bảng Import
        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, int currentUserId);

        Task<bool> UpdateAsync(int id, ProductCreateDto dto);

        Task<bool> DeleteAsync(int id);
    }
}