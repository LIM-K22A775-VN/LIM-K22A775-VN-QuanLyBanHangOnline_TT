using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Products;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;
using QuanLyBanHangOnline.DTO.Generic;
using QuanLyBanHangOnline.Helpers;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(ApplicationDbContext context, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResult<ProductResponseDto>> GetAllAsync(PaginationParams @params)
        {
            // 1. Tạo Query lấy dữ liệu thô (Chưa thực thi SQL)
            // Chúng ta trả về một Anonymous Object chứa cặp Product (p) và Detail (d)
            var query = from p in _context.Product
                        join d in _context.ProductDetail on p.IdSP equals d.IdSP into details
                        from d in details.DefaultIfEmpty()
                        orderby p.IdSP
                        select new { Product = p, Detail = d };

            // 2. Thực thi phân trang để lấy dữ liệu về RAM (In-memory)
            // ToPagedResultAsync sẽ chạy SQL để lấy đúng số bản ghi của trang hiện tại
            var pagedRawData = await query.ToPagedResultAsync(@params.PageNumber, @params.PageSize);

            // 3. Mapping dữ liệu thô sang DTO trên RAM
            // Lúc này dữ liệu đã là List, nên MapToResponse sẽ chạy bình thường không bị lỗi SQL
            var dtos = pagedRawData.Items
                .Select(x => MapToResponse(x.Product, x.Detail))
                .ToList();

            // 4. Trả về kết quả PagedResult mới chứa danh sách DTO
            return new PagedResult<ProductResponseDto>(
                dtos,
                pagedRawData.TotalCount,
                pagedRawData.PageNumber,
                pagedRawData.PageSize
            );
        }
        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            // Tìm sản phẩm kèm theo bản ghi chi tiết của nó
            var product = await _context.Product.FindAsync(id);
            if (product == null) return null;

            var detail = await _context.ProductDetail.FirstOrDefaultAsync(d => d.IdSP == id);

            return MapToResponse(product, detail);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Xử lý ảnh
                string fileName = await ImgHelper.SaveImageAsync(dto.ImageFile, _environment.WebRootPath, "products");

                // 2. Lưu vào bảng Product trước
                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    StockQuantity = dto.StockQuantity,
                    Category = dto.Category,
                    Image = fileName
                };
                _context.Product.Add(product);
                await _context.SaveChangesAsync(); // Lúc này product.IdSP sẽ tự sinh ra

                // 3. Lưu vào bảng ProductDetail
                var detail = new ProductDetail
                {
                    IdSP = product.IdSP, // Lấy ID vừa sinh ra gán sang đây
                    Size = dto.Size ?? "N/A",
                    Color = dto.Color ?? "N/A",
                    Description = dto.Description ?? "",
                    StartTB = 0 // Mặc định đánh giá 0 sao khi mới tạo
                };
                _context.ProductDetail.Add(detail);
                await _context.SaveChangesAsync();

                // Hoàn tất giao dịch
                await transaction.CommitAsync();

                return MapToResponse(product, detail);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, ProductCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Product.FindAsync(id);
                var detail = await _context.ProductDetail.FirstOrDefaultAsync(d => d.IdSP == id);

                if (product == null) return false;

                // Cập nhật bảng Product
                product.Name = dto.Name;
                product.Price = dto.Price;
                product.StockQuantity = dto.StockQuantity;
                product.Category = dto.Category;

                if (dto.ImageFile != null)
                {
                    ImgHelper.DeleteImage(_environment.WebRootPath, "products", product.Image);
                    product.Image = await ImgHelper.SaveImageAsync(dto.ImageFile, _environment.WebRootPath, "products");
                }

                // Cập nhật bảng ProductDetail (Nếu chưa có thì tạo mới, có rồi thì sửa)
                if (detail == null)
                {
                    detail = new ProductDetail { IdSP = id };
                    _context.ProductDetail.Add(detail);
                }
                detail.Size = dto.Size;
                detail.Color = dto.Color;
                detail.Description = dto.Description;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return false;

            // Dùng ImgHelper để xóa cũ và lưu mới
            ImgHelper.DeleteImage(_environment.WebRootPath, "products", product.Image);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private ProductResponseDto MapToResponse(Product p, ProductDetail? d = null)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return new ProductResponseDto
            {
                IdSP = p.IdSP,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Category = p.Category,
                ImageUrl = $"{baseUrl}/images/products/{p.Image}",

                // Gán thêm dữ liệu từ bảng Detail
                Size = d?.Size ?? "",
                Color = d?.Color ?? "",
                Description = d?.Description ?? "",
                StartTB = d?.StartTB ?? 0
            };
        }

       
    }
}
