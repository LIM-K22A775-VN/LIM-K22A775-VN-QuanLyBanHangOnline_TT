using quanlybanhangonline.Import;

namespace QuanLyBanHangOnline.Services.Interfaces
{
    public interface IImportService
    {
        Task<ImportResponseDto> CreateImportAsync(ImportRequestDto dto, int currentUserId);
        Task<IEnumerable<ImportResponseDto>> GetAllImportsAsync(); // Hàm lấy danh sách
        Task<ImportResponseDto?> GetImportByIdAsync(int id);       // Hàm lấy chi tiết
    }
}
