using FluentValidation;
using QuanLyBanHangOnline.DTO.Staffs;

namespace QuanLyBanHangOnline.Validations.Staffs
{
    public class CreatStaffValidator : AbstractValidator<CreatStaffDto>
    {
        public CreatStaffValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng Email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên nhân viên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên không được quá 100 ký tự");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(03|05|07|08|09|01[2|6|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam");

            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Mức lương không được là số âm")
                .When(x => x.Salary.HasValue);

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Vui lòng chọn nhóm quyền (Role) cho nhân viên")
                .GreaterThan(0).WithMessage("Nhóm quyền không hợp lệ");
        }
    }
}