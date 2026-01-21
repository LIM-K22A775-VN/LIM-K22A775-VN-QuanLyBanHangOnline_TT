using static QuanLyBanHangOnline.Constants.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.Review
{
    public class CreateReviewDto
    {
        [Required]
        public int IdSP { get; set; }

        [Required]
        public StarRating Rating { get; set; }

        public string Comment { get; set; } = string.Empty;
    }
}
