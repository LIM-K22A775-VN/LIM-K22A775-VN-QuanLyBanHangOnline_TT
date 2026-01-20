using FluentValidation;
using QuanLyBanHangOnline.DTO.Staffs;

namespace QuanLyBanHangOnline.Validations.Staffs
{
    public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
    {
        public UpdateStaffValidator()
        {
            // FullName: Nếu nhập thì không được để trống và tối đa 100 ký tự
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .MaximumLength(100).WithMessage("Họ tên không được quá 100 ký tự")
                .When(x => x.FullName != null);

            // Phone: Nếu nhập thì phải đúng định dạng số điện thoại Việt Nam
            RuleFor(x => x.Phone)
                .Matches(@"^(03|05|07|08|09|01[2|6|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            // Password: Nếu đổi mật khẩu thì phải từ 6 ký tự trở lên
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Password));

            // Salary: Không được là số âm
            RuleFor(x => x.Salary)
                .GreaterThanOrEqualTo(0).WithMessage("Mức lương không được là số âm")
                .When(x => x.Salary.HasValue);

            // RoleId: Phải là ID hợp lệ nếu có truyền lên
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Nhóm quyền không hợp lệ")
                .When(x => x.RoleId.HasValue);
        }
    }
}