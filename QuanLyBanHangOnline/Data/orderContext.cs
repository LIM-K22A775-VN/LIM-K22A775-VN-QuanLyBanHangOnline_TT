using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class orderContext : DbContext
    {
        public orderContext (DbContextOptions<orderContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.Order> Order { get; set; } = default!;
    }
}
