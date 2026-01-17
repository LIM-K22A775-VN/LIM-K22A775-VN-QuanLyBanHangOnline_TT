namespace QuanLyBanHangOnline.DTO.Generic
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>(); //danh sách dữ liệu chính
        public int TotalCount { get; set; } //Tổng số bản ghi có trong Database
        public int PageNumber { get; set; } //Số thứ tự của trang hiện tại
        public int PageSize { get; set; }  //Số lượng bản ghi hiển thị trên một trang
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
