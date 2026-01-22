
using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class Staff : Account
    {
        public Staff() { RoleType = "Staff"; }
        public string FullName { get; set; }
        public decimal Salary { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
    }
}