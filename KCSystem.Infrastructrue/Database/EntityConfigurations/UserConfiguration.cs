using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KCSystem.Core.Entities;

namespace KCSystem.Infrastructrue.Database.EntityConfigurations
{
    /// <summary>
    /// 管理用户
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
         

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.UserName).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)").HasDefaultValue(""); 
            builder.Property(p => p.UserPassword).IsRequired().HasMaxLength(500).HasColumnType("nvarchar(500)"); 
             
            builder.Property(p => p.AddTime).IsRequired();
            builder.Property(p => p.RealName).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
            builder.Property(p => p.Phone).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
           

            builder.HasOne<User>(s => s.Leard)
                .WithMany(g => g.Children)
                .HasForeignKey(s => s.LeardId);

            builder.HasOne<Role>(s => s.Role)
                .WithMany(g => g.Users)
                .HasForeignKey(s => s.RoleId);

        }
    }
}
