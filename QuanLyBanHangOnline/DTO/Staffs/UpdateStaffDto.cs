namespace QuanLyBanHangOnline.DTO.Staffs
{
    public class UpdateStaffDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // optional: đổi mật khẩu
        public string? Password { get; set; }
        public decimal? Salary { get; set; } // luong
    }
}
