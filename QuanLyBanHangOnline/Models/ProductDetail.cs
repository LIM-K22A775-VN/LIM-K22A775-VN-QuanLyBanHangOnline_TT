using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class ProductDetail
    {
        [Key] // Chỉ định đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Thông thường Id này lấy từ bảng Product sang
        public int IdSP { get; set; }

        public string Size { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public decimal StartTB { get; set; }
    }
}
