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
    public class BackEnumConfiguration : IEntityTypeConfiguration<BackEnum>
    {
        public void Configure(EntityTypeBuilder<BackEnum> builder)
        {

            builder.HasOne<BackEnum>(s => s.Parent)
                .WithMany(g => g.Children)
                .HasForeignKey(s => s.ParentId);

        }
    }
}
