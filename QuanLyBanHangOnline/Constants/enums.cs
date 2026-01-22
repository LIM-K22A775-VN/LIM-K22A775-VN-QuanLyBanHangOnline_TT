using System.ComponentModel.DataAnnotations;

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
        public enum ProductSize
        {
            M, L, XL, XXL
        }

        public enum ProductColor
        {
            DEN, TRANG, DO, XANH_DUONG, XANH_LA, VANG, CAM, TIM, HONG, NAU, XAM, BE
        }

        public enum ProductCategory
        {
            AO_NAM,
            AO_NU,
            QUAN_NAM,
            QUAN_NU,
            PHU_KIEN,
            GIAY_DEP,
        }

        public enum StarRating
        {
            ONE = 1,
            TWO = 2,
            THREE = 3,
            FOUR = 4,
            FIVE = 5
        }
    }
}
