using FluentValidation;
using QuanLyBanHangOnline.DTO.Role;

namespace QuanLyBanHangOnline.Validations.Roles
{
    public class RoleCreateUpdateValidator : AbstractValidator<RoleCreateUpdateDto>
    {
        public RoleCreateUpdateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tên vai trò (Title) không được để trống")
                .MinimumLength(3).WithMessage("Tên vai trò phải có ít nhất 3 ký tự")
                .MaximumLength(50).WithMessage("Tên vai trò không được vượt quá 50 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự");
        }
    }
}