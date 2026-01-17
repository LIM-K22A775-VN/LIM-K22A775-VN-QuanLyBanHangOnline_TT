using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Products;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;
using QuanLyBanHangOnline.DTO.Generic;

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
            var query = _context.Product.AsQueryable();

            // 1. Tính tổng số lượng bản ghi
            var totalCount = await query.CountAsync();

            // 2. Xử lý điều kiện nhập số âm hoặc bằng 0
            int pageNumber = @params.PageNumber < 1 ? 1 : @params.PageNumber;
            int pageSize = @params.PageSize < 1 ? 10 : @params.PageSize;

            // 3. Xử lý điều kiện nếu nhập số trang vượt quá tổng số trang hiện có
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Nếu có dữ liệu mà người dùng yêu cầu trang lớn hơn tổng số trang
            if (totalPages > 0 && pageNumber > totalPages)
            {
                pageNumber = totalPages; // Đưa người dùng về trang cuối cùng
            }

            // 4. Phân trang an toàn
            var products = await query
                .OrderBy(p => p.IdSP)
                .Skip((pageNumber - 1) * pageSize) // Đảm bảo biểu thức này không bao giờ âm
                .Take(pageSize)
                .ToListAsync();

            // 5. Map sang DTO
            var dtos = products.Select(p => MapToResponse(p)).ToList();

            // 6. Trả về đối tượng PagedResult (Dùng pageNumber và pageSize đã xử lý)
            return new PagedResult<ProductResponseDto>(dtos, totalCount, pageNumber, pageSize);
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
                string fileName = await SaveImage(dto.ImageFile);

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
                    DeleteOldImage(product.Image);
                    product.Image = await SaveImage(dto.ImageFile);
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

            DeleteOldImage(product.Image);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- Helper Methods ---
        private async Task<string> SaveImage(IFormFile? file)
        {
            if (file == null) return "default.jpg";

            string wwwRootPath = _environment.WebRootPath;

            // Lấy tên file gốc nhưng KHÔNG lấy đuôi (ví dụ: "tt.jpg" -> "tt")
            string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);

            // 1. Làm sạch tên file (xóa khoảng trắng, ký tự đặc biệt)
            string safeFileName = GenerateSlug(fileNameOnly);

            // 2. Tạo tên file mới: "tt-20260117132559.jpg"
            string extension = Path.GetExtension(file.FileName);
            string fileName = $"{safeFileName}-{DateTime.Now:yyyyMMddHHmmss}{extension}";

            string path = Path.Combine(wwwRootPath, "images/products", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }

        // Hàm hỗ trợ xóa dấu và ký tự đặc biệt
        private string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();
            // Logic đơn giản: thay khoảng trắng bằng dấu gạch ngang
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-");
            return str;
        }

        private void DeleteOldImage(string fileName)
        {
            if (fileName == "default.jpg") return;
            string path = Path.Combine(_environment.WebRootPath, "images/products", fileName);
            if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
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
                StartTB = d?.StartTB ?? 5
            };
        }

       
    }
}
