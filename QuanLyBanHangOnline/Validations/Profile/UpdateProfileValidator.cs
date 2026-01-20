using FluentValidation;
using QuanLyBanHangOnline.DTO.Profile;

namespace QuanLyBanHangOnline.Validations.Profile
{
    public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileValidator()
        {
            // Validate Họ tên
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MinimumLength(2).WithMessage("Họ tên phải có ít nhất 2 ký tự")
                .MaximumLength(50).WithMessage("Họ tên không được vượt quá 50 ký tự");

            // Validate Số điện thoại (Định dạng Việt Nam)
            RuleFor(x => x.Phone)
                .Matches(@"^(0[3|5|7|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam (10 số)")
                .When(x => !string.IsNullOrEmpty(x.Phone)); // Chỉ validate nếu người dùng có nhập số điện thoại

            // Validate Địa chỉ
            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Địa chỉ không được vượt quá 200 ký tự");
        }
    }
}
