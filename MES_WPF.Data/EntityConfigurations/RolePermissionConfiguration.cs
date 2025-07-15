using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // 表名
            builder.ToTable("RolePermissions");

            // 主键
            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.Id).ValueGeneratedOnAdd();

            // 关系配置 - 如果需要配置外键关系的话，取消下面的注释并修改
            // builder.HasOne<Role>().WithMany().HasForeignKey(rp => rp.RoleId);
            // builder.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId);

            // 唯一索引 - 同一角色不能拥有相同的权限两次
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
            
            // 普通索引
            builder.HasIndex(rp => rp.RoleId);
            builder.HasIndex(rp => rp.PermissionId);
        }
    }
} 