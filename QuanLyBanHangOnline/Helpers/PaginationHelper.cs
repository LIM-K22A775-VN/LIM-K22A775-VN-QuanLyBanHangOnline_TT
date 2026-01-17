using Microsoft.EntityFrameworkCore;
using QuanLyBanHangOnline.DTO.Generic;

namespace QuanLyBanHangOnline.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize)
        {
            // 1. Tính tổng số bản ghi
            var totalCount = await query.CountAsync();

            // 2. Xử lý an toàn (Chống số âm)
            int validPageNumber = pageNumber < 1 ? 1 : pageNumber;
            int validPageSize = pageSize < 1 ? 10 : pageSize;

            // 3. Chặn trang quá thực tế
            int totalPages = (int)Math.Ceiling(totalCount / (double)validPageSize);
            if (totalPages > 0 && validPageNumber > totalPages)
            {
                validPageNumber = totalPages;
            }

            // 4. Thực hiện phân trang
            var items = await query
                .Skip((validPageNumber - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync();

            return new PagedResult<T>(items, totalCount, validPageNumber, validPageSize);
        }


    }
}
