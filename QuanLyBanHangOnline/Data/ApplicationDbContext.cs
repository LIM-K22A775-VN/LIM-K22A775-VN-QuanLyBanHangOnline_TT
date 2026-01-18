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

    public DbSet<QuanLyBanHangOnline.Models.Role> Role { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>(); // Lưu "ChoXacNhan"   

        modelBuilder.Entity<ProductDetail>()
        .HasOne<Product>()
        .WithOne()
        .HasForeignKey<ProductDetail>(pd => pd.IdSP);
    }
}
