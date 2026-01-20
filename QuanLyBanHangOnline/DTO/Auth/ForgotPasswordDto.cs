namespace QuanLyBanHangOnline.DTO.Auth
{
    // Bước 1: Nhập Email để nhận mã OTP qua Gmail
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    // Bước 2: Nhập OTP để xác thực (Vẫn cần Email ở đây để tìm mã OTP trong DB)
    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    // Bước 3: Đặt lại mật khẩu (Không cần Email vì sẽ lấy từ Token xác thực)
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}