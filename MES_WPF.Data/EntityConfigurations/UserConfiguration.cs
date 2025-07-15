using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // 表名
            builder.ToTable("Users");

            // 主键
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(100);
            builder.Property(u => u.RealName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).HasMaxLength(100);
            builder.Property(u => u.Mobile).HasMaxLength(20);
            builder.Property(u => u.Avatar).HasMaxLength(200);
            builder.Property(u => u.LastLoginIp).HasMaxLength(50);
            builder.Property(u => u.Remark).HasMaxLength(200);

            // 唯一索引
            builder.HasIndex(u => u.Username).IsUnique();
            
            // 普通索引
            builder.HasIndex(u => u.EmployeeId);
            builder.HasIndex(u => u.Email);
            builder.HasIndex(u => u.Mobile);
            builder.HasIndex(u => u.Status);
        }
    }
} 