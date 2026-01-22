using QuanLyBanHangOnline.Models;
using System.ComponentModel.DataAnnotations;

namespace quanlybanhangonline.Models
{
    public class Admin : Account {

        public Admin() { RoleType = "Admin"; }
    }
}