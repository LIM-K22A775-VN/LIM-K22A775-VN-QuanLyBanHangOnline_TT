using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class QuanLyBanHangOnlineContext : DbContext
    {
        public QuanLyBanHangOnlineContext (DbContextOptions<QuanLyBanHangOnlineContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.ProductDetail> ProductDetail { get; set; } = default!;
    }
}
