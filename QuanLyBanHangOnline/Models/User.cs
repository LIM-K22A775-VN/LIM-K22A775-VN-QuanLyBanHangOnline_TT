using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations;

namespace quanlybanhangonline.Models
{
    public class User : Account
    {
        public User() { RoleType = "User"; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}