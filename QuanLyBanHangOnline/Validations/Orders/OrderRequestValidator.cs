using FluentValidation;
using quanlybanhangonline.Models.DTOs;

namespace QuanLyBanHangOnline.Validations.Orders
{
    public class OrderRequestValidator : AbstractValidator<OrderRequestDto>
    {
        public OrderRequestValidator()
        {
            // 1. Validate danh sách sản phẩm
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Đơn hàng phải có ít nhất một sản phẩm.")
                .Must(x => x != null && x.Count > 0).WithMessage("Danh sách sản phẩm không được để trống.");

            // Tự động gọi OrderDetailRequestValidator để kiểm tra từng IdSP và Quantity
            RuleForEach(x => x.Items).SetValidator(new OrderDetailRequestValidator());

            // 2. Validate thông tin người nhận
            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("Tên người nhận không được để trống.")
                .MaximumLength(200).WithMessage("Tên người nhận không quá 200 ký tự.");

            RuleFor(x => x.ReceiverPhone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^(03|05|07|08|09|01[2|6|8|9])([0-9]{8})$")
                .WithMessage("Số điện thoại không đúng định dạng Việt Nam.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống.")
                .MaximumLength(500).WithMessage("Địa chỉ không quá 500 ký tự.");
        }
    }
}