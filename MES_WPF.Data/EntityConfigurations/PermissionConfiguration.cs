using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            // 表名
            builder.ToTable("Permissions");

            // 主键
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(p => p.PermissionCode).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PermissionName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Path).HasMaxLength(200);
            builder.Property(p => p.Component).HasMaxLength(200);
            builder.Property(p => p.Icon).HasMaxLength(100);
            builder.Property(p => p.Remark).HasMaxLength(200);

            // 自引用关系 - 父子权限
            builder.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .IsRequired(false) // 允许为空，表示顶级权限
                .OnDelete(DeleteBehavior.Restrict); // 防止级联删除

            // 唯一索引
            builder.HasIndex(p => p.PermissionCode).IsUnique();
            
            // 普通索引
            builder.HasIndex(p => p.PermissionType);
            builder.HasIndex(p => p.ParentId);
            builder.HasIndex(p => p.Status);
        }
    }
} 