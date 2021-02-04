using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using KCSystem.Core.Entities;

namespace KCSystem.Infrastructrue.Database.EntityConfigurations
{
   public class RolePowerConfiguration : IEntityTypeConfiguration<RolePower>
    {
        public void Configure(EntityTypeBuilder<RolePower> builder)
        {
            builder.Property(p => p.PowerName).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)").HasDefaultValue("");
            builder.Property(p => p.Action).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)").HasDefaultValue("");
            
            builder.HasOne<Role>(s => s.Role)
               .WithMany(g => g.Powers)
               .HasForeignKey(s => s.RoleId);
        }
    }
}
