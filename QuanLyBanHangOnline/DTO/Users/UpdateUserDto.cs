namespace QuanLyBanHangOnline.DTO.Users
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // optional: đổi mật khẩu
        public string? Password { get; set; }
    }
}
