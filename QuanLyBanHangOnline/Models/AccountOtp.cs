using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybanhangonline.Models
{
    public class AccountOtp
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;

        [ForeignKey("Email")]
        public virtual Account? Account { get; set; }
    }
}