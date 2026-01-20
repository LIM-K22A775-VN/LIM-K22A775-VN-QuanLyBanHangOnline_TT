using FluentValidation;
using QuanLyBanHangOnline.DTO.Generic;

namespace QuanLyBanHangOnline.Validations.Generic
{
    public class PaginationValidator : AbstractValidator<PaginationParams>
    {
        public PaginationValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Số trang phải bắt đầu từ 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Số lượng bản ghi mỗi trang phải từ 1 đến 50.");
        }
    }
}