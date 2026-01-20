using FluentValidation;
using QuanLyBanHangOnline.DTO.Auth;

namespace QuanLyBanHangOnline.Validations.Profile
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            // Validate Mật khẩu cũ
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu cũ");

            // Validate Mật khẩu mới
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Vui lòng nhập mật khẩu mới")
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
                .NotEqual(x => x.OldPassword).WithMessage("Mật khẩu mới không được trùng với mật khẩu cũ");

            // Validate Xác nhận mật khẩu
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Vui lòng xác nhận lại mật khẩu mới")
                .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu mới");
        }
    }
}
