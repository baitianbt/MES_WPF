using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // 表名
            builder.ToTable("Roles");

            // 主键
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(r => r.RoleCode).IsRequired().HasMaxLength(50);
            builder.Property(r => r.RoleName).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Remark).HasMaxLength(200);

            // 唯一索引
            builder.HasIndex(r => r.RoleCode).IsUnique();
            
            // 普通索引
            builder.HasIndex(r => r.RoleType);
            builder.HasIndex(r => r.Status);
        }
    }
} 