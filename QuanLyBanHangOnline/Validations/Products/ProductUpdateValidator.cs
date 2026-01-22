using FluentValidation;
using QuanLyBanHangOnline.DTO.Products;

namespace QuanLyBanHangOnline.Validations.Products
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            // IdSP là bắt buộc để biết đang cập nhật sản phẩm nào
            RuleFor(x => x.IdSP)
                .NotEmpty().WithMessage("ID sản phẩm không được để trống");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .Length(3, 200).WithMessage("Tên sản phẩm phải từ 3 đến 200 ký tự");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(1000).WithMessage("Giá sản phẩm tối thiểu là 1,000đ");

            RuleFor(x => x.ImportPrice)
                .GreaterThanOrEqualTo(1000).WithMessage("Giá sản phẩm tối thiểu là 1,000đ"); //Lớn hơn hoặc bằng

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng kho không được âm");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Vui lòng chọn danh mục sản phẩm");

            // Validate File Ảnh (Chỉ kiểm tra KHI CÓ gửi file lên)
            RuleFor(x => x.ImageFile)
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage("Kích thước ảnh mới không được vượt quá 5MB")
                .Must(file => file == null || IsAllowedExtension(file))
                .WithMessage("Định dạng ảnh mới phải là .jpg, .jpeg, .png hoặc .webp");
        }

        // Tận dụng lại hàm check đuôi file
        private bool IsAllowedExtension(IFormFile file)
        {
            if (file == null) return true;
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            return extensions.Contains(ext);
        }
    }
}