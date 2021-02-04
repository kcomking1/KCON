using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using KCSystem.Core.Entities;

namespace KCSystem.Infrastructrue.Database.EntityConfigurations
{
    /// <summary>
    /// 模板管理
    /// </summary>
    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {        

            builder.Property(p => p.Scenarios).HasMaxLength(50).HasColumnType("nvarchar(50)").HasDefaultValue("");

            builder.Property(p => p.Description).HasMaxLength(100).HasColumnType("nvarchar(100)").HasDefaultValue("");

           

            builder.HasOne<User>(s => s.User)
                .WithMany(g => g.Logs)
                .HasForeignKey(s => s.UserId);


        }
    }
}
