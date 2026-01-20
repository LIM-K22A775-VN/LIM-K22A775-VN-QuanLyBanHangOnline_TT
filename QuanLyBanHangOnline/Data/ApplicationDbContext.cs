using Humanizer;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

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

    public DbSet<AccountOtp> AccountOtps { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>(); // Lưu "ChoXacNhan"   

        modelBuilder.Entity<ProductDetail>()
        .HasOne<Product>()
        .WithOne()
        .HasForeignKey<ProductDetail>(pd => pd.IdSP);

        // --- SEED DATA: Tạo tài khoản Admin mặc định ---
        modelBuilder.Entity<Admin>().HasData(new Admin
        {
            IdAdmin = 1,
            Email = "admin99@gmail.com",       
            Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
        });


        // Cấu hình quan hệ 1-1 giữa Product và ProductDetail
        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductDetail)
            .WithOne(d => d.Product)
            .HasForeignKey<ProductDetail>(d => d.IdSP); // Xác định ProductDetail là bên phụ thuộc

        // Nếu bạn muốn lưu Enum dưới dạng String trong DB (Tùy chọn)
        modelBuilder.Entity<ProductDetail>()
            .Property(d => d.Size)
            .HasConversion<string>();

        modelBuilder.Entity<ProductDetail>()
            .Property(d => d.Color)
            .HasConversion<string>();

        modelBuilder.Entity<Product>()
            .Property(p => p.Category)
            .HasConversion<string>();


    }
}
