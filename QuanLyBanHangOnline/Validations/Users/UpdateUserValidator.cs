using FluentValidation;
using QuanLyBanHangOnline.DTO.Users;

namespace QuanLyBanHangOnline.Validations.Users
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            // FullName: Nếu gửi lên thì không được để trống
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên không vượt quá 100 ký tự")
                .When(x => x.FullName != null);

            // Phone: Kiểm tra định dạng số điện thoại Việt Nam
            RuleFor(x => x.Phone)
                .Matches(@"^(03|05|07|08|09|01[2|6|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            // Address: Giới hạn độ dài địa chỉ
            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Địa chỉ không vượt quá 500 ký tự")
                .When(x => x.Address != null);

            // Password: Quy tắc bảo mật khi đổi mật khẩu mới
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu mới phải có ít nhất một chữ cái viết hoa")
                .Matches(@"[0-9]").WithMessage("Mật khẩu mới phải có ít nhất một chữ số")
                .When(x => !string.IsNullOrEmpty(x.Password));
        }
    }
}