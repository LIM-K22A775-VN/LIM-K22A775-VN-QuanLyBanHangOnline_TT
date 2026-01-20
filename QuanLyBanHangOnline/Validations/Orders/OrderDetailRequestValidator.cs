using FluentValidation;
using QuanLyBanHangOnline.DTO.OrderDetailRequestDto;

namespace QuanLyBanHangOnline.Validations.Orders
{
    public class OrderDetailRequestValidator : AbstractValidator<OrderDetailRequestDto>
    {
        public OrderDetailRequestValidator()
        {
            RuleFor(x => x.IdSP)
                .NotEmpty().WithMessage("ID sản phẩm không được để trống")
                .GreaterThan(0).WithMessage("ID sản phẩm không hợp lệ");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng mua phải ít nhất là 1");
        }
    }
}