using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class Import
    {
        [Key]
        public int IdImport { get; set; }

        public DateTime ImportDate { get; set; } = DateTime.Now;

        [Required]
        public int IdStaff { get; set; }

        public decimal TotalCost { get; set; } // Tổng tiền của cả phiếu nhập

        [ForeignKey("IdStaff")]
        public virtual Staff? Staff { get; set; }

        // Quan hệ 1-N với Chi tiết nhập
        public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
    }
}