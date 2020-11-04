using Microsoft.EntityFrameworkCore;
using MyTest.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTest.Entity
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext() { }
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
          : base(options) { }

        public DbSet<AuthSysUserToken> SysUserTokens { get; set; }

        public DbSet<AuthSysUser> Users { get; set; }

    }
}
