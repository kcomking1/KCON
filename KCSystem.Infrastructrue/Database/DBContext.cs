using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore; 
using KCSystem.Core.Entities;
using KCSystem.Core.SqlModel;

namespace KCSystem.Infrastructrue.Database
{
    public class KCDBContext :DbContext
    {
        public KCDBContext(DbContextOptions<KCDBContext> option) : base(option)
        {
            //Add-Migration Initial
            //Update-Database -Verbose
            // Script-Migration  -From:"20190403085254_product.cs" -To
            //remove-migration

        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Core.Entities.Module> Modules { get; set; } 
        public DbSet<RolePower> RolePowers { get; set; }
        public DbSet<BackEnum> BackEnums { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
         

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
