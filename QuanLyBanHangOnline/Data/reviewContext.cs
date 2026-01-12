using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class reviewContext : DbContext
    {
        public reviewContext (DbContextOptions<reviewContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.Review> Review { get; set; } = default!;
    }
}
