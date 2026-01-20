namespace QuanLyBanHangOnline.DTO.Staffs;

public class CreatStaffDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public decimal? Salary { get; set; }
    public int? RoleId { get; set; }
}