using FluentValidation;
using QuanLyBanHangOnline.DTO.Review;

namespace QuanLyBanHangOnline.Validations.Reviews
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            // Kiểm tra Id sản phẩm
            RuleFor(x => x.IdSP)
                .NotEmpty().WithMessage("Mã sản phẩm không được để trống.");

            // Kiểm tra số sao (Rating)
            RuleFor(x => x.Rating)
                .IsInEnum().WithMessage("Số sao đánh giá không hợp lệ (phải từ 1 đến 5).");

            // Kiểm tra nội dung bình luận
            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Vui lòng nhập nội dung đánh giá.")
                .MinimumLength(10).WithMessage("Nội dung đánh giá phải có ít nhất 10 ký tự.")
                .MaximumLength(500).WithMessage("Nội dung đánh giá không được vượt quá 500 ký tự.");
        }
    }
}