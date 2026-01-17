namespace QuanLyBanHangOnline.DTO.Generic
{
    public class PaginationParams
    {
        public int PageNumber { get; set; } = 1; // Mặc định lấy trang 1

        private int _pageSize = 10; // Số lượng mặc định
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 50) ? 50 : value; // Giới hạn tối đa 50 để tránh quá tải server
        }
    }
}