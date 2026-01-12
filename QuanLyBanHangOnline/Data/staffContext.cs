using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class staffContext : DbContext
    {
        public staffContext (DbContextOptions<staffContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.Staff> Staff { get; set; } = default!;
    }
}
