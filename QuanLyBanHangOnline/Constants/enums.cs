namespace QuanLyBanHangOnline.Constants
{
    public class Enums
    {
        public enum OrderStatus
        {
            ChoXacNhan = 0,    // Chờ xác nhận
            DaXacNhan = 1,     // Đã xác nhận
            DaVanChuyen = 2,   // Đã vận chuyển
            DaNhanHang = 3,    // Đã nhận hàng
            DaHuy = 4          // (Nên thêm trạng thái Hủy đơn)
        }

    }
}
