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
            [Display(Name = "Áo Nam")]
            AO_NAM,
            [Display(Name = "Áo Nữ")]
            AO_NU,
            [Display(Name = "Quần Nam")]
            QUAN_NAM,
            [Display(Name = "Quần Nữ")]
            QUAN_NU,
            [Display(Name = "Phụ Kiện")]
            PHU_KIEN,
            [Display(Name = "Giày Dép")]
            GIAY_DEP
        }

    }
}
