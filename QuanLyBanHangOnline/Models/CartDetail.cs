using quanlybanhangonline.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class CartDetail
    {
        [Key]
        public int IdCartDetail { get; set; }

        [Required]
        public int IdCart { get; set; }

        [Required]
        public int IdSP { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1")]
        public int Quantity { get; set; }

        [ForeignKey("IdCart")]
        public virtual Cart? Cart { get; set; }

        [ForeignKey("IdSP")]
        public virtual Product? Product { get; set; }
    }
}