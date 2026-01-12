using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class orderdetailContext : DbContext
    {
        public orderdetailContext (DbContextOptions<orderdetailContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.OrderDetail> OrderDetail { get; set; } = default!;
    }
}
