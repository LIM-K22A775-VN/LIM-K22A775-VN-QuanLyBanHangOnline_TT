using FluentValidation;
using QuanLyBanHangOnline.DTO.Users;

namespace QuanLyBanHangOnline.Validations.Users
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .Matches(@"[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất một chữ cái viết hoa")
                .Matches(@"[0-9]").WithMessage("Mật khẩu phải chứa ít nhất một chữ số");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Vui lòng nhập họ tên đầy đủ")
                .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(03|05|07|08|09|01[2|6|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không hợp lệ (phải đúng định dạng Việt Nam)");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Địa chỉ không được vượt quá 500 ký tự");
        }
    }
}