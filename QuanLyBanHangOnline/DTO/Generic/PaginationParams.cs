namespace QuanLyBanHangOnline.DTO.Generic
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1; // Mặc định lấy trang 1

        public int PageSize { get; set; } = 10; // Số lượng mặc định
    }
}