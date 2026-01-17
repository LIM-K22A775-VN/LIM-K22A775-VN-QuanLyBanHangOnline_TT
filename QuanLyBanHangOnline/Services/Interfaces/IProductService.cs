using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.DTO.Products;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductResponseDto>> GetAllAsync(PaginationParams @params);
        Task<ProductResponseDto?> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(int id, ProductCreateDto dto); // Dùng lại CreateDto hoặc tạo UpdateDto riêng
        Task<bool> DeleteAsync(int id);
    }
}
