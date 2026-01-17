namespace QuanLyBanHangOnline.Helpers
{
    public static class ImgHelper
    {
        public static async Task<string> SaveImageAsync(IFormFile? file, string webRootPath, string folderName)
        {
            if (file == null) return "default.jpg";

            // 1. Làm sạch tên file
            string fileNameOnly = Path.GetFileNameWithoutExtension(file.FileName);
            string safeFileName = GenerateSlug(fileNameOnly);
            string extension = Path.GetExtension(file.FileName);

            // 2. Tạo tên file duy nhất bằng timestamp
            string fileName = $"{safeFileName}-{DateTime.Now:yyyyMMddHHmmss}{extension}";

            // 3. Thiết lập đường dẫn linh hoạt (images/products hoặc images/avatars...)
            string folderPath = Path.Combine(webRootPath, "images", folderName);

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fullPath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }

        public static void DeleteImage(string webRootPath, string folderName, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName == "default.jpg") return;

            string path = Path.Combine(webRootPath, "images", folderName, fileName);
            if (File.Exists(path)) File.Delete(path);
        }

        public static string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();
            // Thay khoảng trắng và ký tự đặc biệt bằng dấu gạch ngang
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", "-").Trim();
            return str;
        }
    }
}
