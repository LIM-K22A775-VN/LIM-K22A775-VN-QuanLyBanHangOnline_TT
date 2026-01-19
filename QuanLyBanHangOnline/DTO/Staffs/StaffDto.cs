namespace QuanLyBanHangOnline.DTO.Staffs
{
    public class StaffDto
    {
        public int IdStaff { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal? Salary { get; set; } // luong

        // Bổ sung thông tin Role
        public int? RoleId { get; set; }
        public string? RoleName { get; set; } // Hiển thị "Quản trị viên", "Nhân viên"...
    }
}
