using FluentValidation;
using QuanLyBanHangOnline.DTO.Auth;

namespace QuanLyBanHangOnline.Validations.Profile
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Vui lòng nhập Email để khôi phục mật khẩu")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");
        }
    }
}
