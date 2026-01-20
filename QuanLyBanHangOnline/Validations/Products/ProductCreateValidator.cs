using FluentValidation;
using QuanLyBanHangOnline.DTO.Products;

namespace QuanLyBanHangOnline.Validations.Products
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .Length(3, 200).WithMessage("Tên sản phẩm phải từ 3 đến 200 ký tự");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(1000).WithMessage("Giá sản phẩm tối thiểu là 1,000đ"); //Lớn hơn hoặc bằng

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng kho không được âm");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Vui lòng chọn danh mục sản phẩm");

            // Validate File Ảnh nâng cao
            RuleFor(x => x.ImageFile)
                //(Custom Validation) bằng biểu thức Lambda. file.Length : kb  1 KB = 1024 Bytes. 1 MB = 1024 KB 1 GB = 1024 MB
                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("Kích thước ảnh không được vượt quá 5MB")
                .Must(file => IsAllowedExtension(file)).WithMessage("Định dạng ảnh phải là .jpg, .jpeg hoặc .png");

            RuleFor(x => x.Size).NotEmpty().WithMessage("Vui lòng nhập kích thước");
            RuleFor(x => x.Color).NotEmpty().WithMessage("Vui lòng nhập màu sắc");
        }

        // Hàm phụ kiểm tra đuôi file
        private bool IsAllowedExtension(IFormFile file)
        {
            if (file == null) return true; //không bắt buộc up ảnh
            var extensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower(); //Path.GetExtension() " tìm tên cuối cùng 
            return extensions.Contains(ext);
        }
    }
}