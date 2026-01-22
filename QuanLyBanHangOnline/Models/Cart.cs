using quanlybanhangonline.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Model
{
    public class Cart
    {
        [Key]
        public int IdCart { get; set; }

        [Required]
        public int IdUser { get; set; }

        // Trạng thái: 1 - Có hàng, 0 - Rỗng
        public int Status { get; set; } = 0;

        [ForeignKey("IdUser")]
        public virtual User? User { get; set; }

        // Quan hệ 1-N: Một giỏ hàng có nhiều chi tiết sản phẩm
        public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}