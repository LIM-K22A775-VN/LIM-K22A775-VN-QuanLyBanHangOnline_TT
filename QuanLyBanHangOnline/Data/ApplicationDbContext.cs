using Humanizer;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Model;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admin> Admin { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<User> User { get; set; }

    public DbSet<Product> Product { get; set; }

    public DbSet<ProductDetail> ProductDetail { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderDetail> OrderDetail { get; set; }
    public DbSet<Review> Review { get; set; }
    public DbSet<Role> Role { get; set; }

    public DbSet<Cart> Cart { get; set; } = default!;

    public DbSet<CartDetail> CartDetail { get; set; } = default!;

    public DbSet<Import> Import { get; set; } = default!;

    public DbSet<ImportDetail> ImportDetail { get; set; } = default!;
    public DbSet<AccountOtp> AccountOtps { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 1. CẤU HÌNH KẾ THỪA TPT (Table-Per-Type) ---
        // Điều này tạo ra các bảng riêng biệt kết nối qua IdAccount
        modelBuilder.Entity<Account>().ToTable("Accounts");
        modelBuilder.Entity<Admin>().ToTable("Admins");
        modelBuilder.Entity<Staff>().ToTable("Staffs");
        modelBuilder.Entity<User>().ToTable("Users");

        // --- 2. CẤU HÌNH PHIẾU NHẬP (IMPORT) ---
        // Bây giờ Import chỉ cần trỏ đến Account (có thể là Admin hoặc Staff)
        modelBuilder.Entity<Import>()
            .HasOne(i => i.Account)
            .WithMany()
            .HasForeignKey(i => i.IdAccount)
            .OnDelete(DeleteBehavior.Restrict);

        // --- 3. SEED DATA CHO ADMIN (Cập nhật theo cấu trúc mới) ---
        // Lưu ý: Password nên được hash trước khi đưa vào HasData
        modelBuilder.Entity<Admin>().HasData(new Admin
        {
            IdAccount = 1, // ID chung từ Account
            Email = "admin99@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("123456"),
            RoleType = "Admin"
        });

        // --- 4. CẤU HÌNH CÁC QUAN HỆ KHÁC ---

        // Quan hệ 1-1 giữa Product và ProductDetail
        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductDetail)
            .WithOne(d => d.Product)
            .HasForeignKey<ProductDetail>(d => d.IdSP);

        // Chuyển đổi Enum sang String để lưu vào DB
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<ProductDetail>().Property(d => d.Size).HasConversion<string>();
        modelBuilder.Entity<ProductDetail>().Property(d => d.Color).HasConversion<string>();
        modelBuilder.Entity<Product>().Property(p => p.Category).HasConversion<string>();

        // Giỏ hàng (Cart) và Chi tiết giỏ hàng (CartDetail)
        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.IdUser)
            .IsUnique();

        modelBuilder.Entity<CartDetail>()
            .HasOne(cd => cd.Cart)
            .WithMany(c => c.CartDetails)
            .HasForeignKey(cd => cd.IdCart)
            .OnDelete(DeleteBehavior.Cascade);

        // Review liên kết với bảng User cụ thể
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.IdUser)
            .OnDelete(DeleteBehavior.Restrict);

        // Cấu hình liên kết OTP qua Email thay vì ID
        modelBuilder.Entity<AccountOtp>()
            .HasOne(tp => tp.Account)
            .WithMany() // Một tài khoản có thể có nhiều yêu cầu OTP theo thời gian
            .HasPrincipalKey(a => a.Email) // Xác định Email là cột tham chiếu bên bảng Account
            .HasForeignKey(tp => tp.Email)  // Xác định Email là cột khóa ngoại bên bảng AccountOtp
            .OnDelete(DeleteBehavior.Cascade); // Xóa tài khoản thì xóa luôn các mã OTP liên quan
    }
}
