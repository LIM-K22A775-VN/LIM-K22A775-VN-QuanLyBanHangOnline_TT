using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class productContext : DbContext
    {
        public productContext (DbContextOptions<productContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.Product> Product { get; set; } = default!;
    }
}
