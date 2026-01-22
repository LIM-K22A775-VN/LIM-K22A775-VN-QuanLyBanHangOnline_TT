using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class ImportDetail
    {
        [Key]
        public int IdImportDetail { get; set; }

        [Required]
        public int IdImport { get; set; }

        [Required]
        public int IdSP { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal ImportPrice { get; set; } // Giá nhập của 1 sản phẩm

        [ForeignKey("IdImport")]
        public virtual Import? Import { get; set; }

        [ForeignKey("IdSP")]
        public virtual Product? Product { get; set; }
    }
}