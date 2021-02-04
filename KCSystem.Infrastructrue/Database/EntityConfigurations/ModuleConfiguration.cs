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
    public class ModuleConfiguration: IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {        

            builder.Property(p => p.ModuleName).HasMaxLength(50).HasColumnType("nvarchar(50)").HasDefaultValue("");

            builder.Property(p => p.Icon).HasMaxLength(100).HasColumnType("nvarchar(100)").HasDefaultValue("");

            builder.Property(p => p.ModuleUrl).IsRequired().HasMaxLength(500).HasColumnType("nvarchar(500)").HasDefaultValue("");

            builder.Property(p => p.ModuleSortIndex).IsRequired();

            builder.Property(p => p.ModuleDesc).HasMaxLength(100).HasColumnType("nvarchar(100)").HasDefaultValue("");

            builder.Property(p => p.ModuleUse).IsRequired().HasDefaultValue(false);

            builder.HasOne<Module>(s => s.Parent)
                .WithMany(g => g.Children)
                .HasForeignKey(s => s.ParentId);


        }
    }
}
