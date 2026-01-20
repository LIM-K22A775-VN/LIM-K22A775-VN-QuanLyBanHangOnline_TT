using FluentValidation;
using QuanLyBanHangOnline.DTO.Roles;

namespace QuanLyBanHangOnline.Validations.Roles
{
    public class RoleUpdateValidator : AbstractValidator<RoleUpdateDto>
    {
        public RoleUpdateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên vai trò không được để trống")
                .Length(3, 50).WithMessage("Tên vai trò phải từ 3 đến 50 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");
        }
    }
}
