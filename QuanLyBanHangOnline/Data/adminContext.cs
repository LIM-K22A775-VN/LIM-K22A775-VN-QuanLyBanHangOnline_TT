using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;

namespace QuanLyBanHangOnline.Data
{
    public class adminContext : DbContext
    {
        public adminContext (DbContextOptions<adminContext> options)
            : base(options)
        {
        }

        public DbSet<quanlybanhangonline.Models.Admin> Admin { get; set; } = default!;
    }
}
