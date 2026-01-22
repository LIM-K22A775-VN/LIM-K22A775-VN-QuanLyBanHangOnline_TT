using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Import;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Services.Interfaces;

namespace QuanLyBanHangOnline.Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly ApplicationDbContext _context;

        public ImportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ImportResponseDto> CreateImportAsync(ImportRequestDto dto, int currentUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Khởi tạo đối tượng Import (Phiếu nhập)
                var import = new Import
                {
                    IdAccount = currentUserId,
                    ImportDate = DateTime.Now,
                    TotalCost = dto.Items.Sum(x => x.Quantity * x.ImportPrice),
                    ImportDetails = new List<ImportDetail>()
                };

                // 2. Xử lý từng món hàng nhập vào
                foreach (var item in dto.Items)
                {
                    var product = await _context.Product.FindAsync(item.IdSP);
                    if (product == null)
                    {
                        throw new Exception($"Sản phẩm có ID {item.IdSP} không tồn tại.");
                    }

                    // Cập nhật số lượng tồn kho và giá nhập mới nhất
                    product.StockQuantity += item.Quantity;
                    product.ImportPrice = item.ImportPrice;

                    import.ImportDetails.Add(new ImportDetail
                    {
                        IdSP = item.IdSP,
                        Quantity = item.Quantity,
                        ImportPrice = item.ImportPrice
                    });
                }

                _context.Import.Add(import);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Nạp thêm thông tin Account và Product để Mapping đầy đủ dữ liệu
                await _context.Entry(import).Reference(i => i.Account).LoadAsync();
                foreach (var detail in import.ImportDetails)
                {
                    await _context.Entry(detail).Reference(d => d.Product).LoadAsync();
                }

                return MapToResponseDto(import);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ĐÂY LÀ PHẦN BẠN ĐANG THIẾU: Logic chuyển đổi từ Entity sang DTO
        private ImportResponseDto MapToResponseDto(Import import)
        {
            return new ImportResponseDto
            {
                IdImport = import.IdImport,
                ImportDate = import.ImportDate,
                IdAccount = import.IdAccount,
                // Lấy Email từ bảng Accounts làm tên người nhập
                Email = import.Account?.Email ?? "N/A",
                TotalCost = import.TotalCost,
                Details = import.ImportDetails.Select(d => new ImportDetailResponseDto
                {
                    IdImportDetail = d.IdImportDetail,
                    IdSP = d.IdSP,
                    ProductName = d.Product?.Name ?? "Sản phẩm không xác định",
                    Quantity = d.Quantity,
                    ImportPrice = d.ImportPrice
                }).ToList()
            };
        }

        // Lấy toàn bộ danh sách phiếu nhập
        public async Task<IEnumerable<ImportResponseDto>> GetAllImportsAsync()
        {
            var imports = await _context.Import
                .Include(i => i.Account)
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(i => i.ImportDate)
                .ToListAsync();

            return imports.Select(i => MapToResponseDto(i));
        }

        // Lấy chi tiết một phiếu nhập theo ID
        public async Task<ImportResponseDto?> GetImportByIdAsync(int id)
        {
            var import = await _context.Import
                .Include(i => i.Account)
                .Include(i => i.ImportDetails)
                    .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(i => i.IdImport == id);

            return import == null ? null : MapToResponseDto(import);
        }
    }
}