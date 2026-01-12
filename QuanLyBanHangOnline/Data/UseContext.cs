using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class UseContext : DbContext
    {
        public UseContext (DbContextOptions<UseContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.User> User { get; set; } = default!;
    }
}
