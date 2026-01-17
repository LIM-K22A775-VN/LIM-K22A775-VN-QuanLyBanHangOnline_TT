using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHangOnline.DTO.Staffs;

public class CreatStaffDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal? Salary { get; set; } // luong
}
