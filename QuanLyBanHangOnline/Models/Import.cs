using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class Import
    {
        [Key]
        public int IdImport { get; set; }
        public DateTime ImportDate { get; set; } = DateTime.Now;

        // Trỏ đến ID chung của lớp cha Account
        public int IdAccount { get; set; }
        public decimal TotalCost { get; set; }

        [ForeignKey("IdAccount")]
        public virtual Account? Account { get; set; }

        public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();
    }
}