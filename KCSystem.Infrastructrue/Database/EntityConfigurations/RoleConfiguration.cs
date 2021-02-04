using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCSystem.Core.Entities;

namespace KCSystem.Infrastructrue.Database.EntityConfigurations
{
    /// <summary>
    /// 权限
    /// </summary>
   public class RoleConfiguration : IEntityTypeConfiguration<Role>
    { 
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)").HasDefaultValue("");
            builder.Property(p => p.Remark).HasMaxLength(200).HasColumnType("nvarchar(200)").HasDefaultValue("");

            builder.HasOne<Role>(s => s.Parent)
                .WithMany(g => g.Children)
                .HasForeignKey(s => s.ParentId);


        }
    }
}
