using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // 表名
            builder.ToTable("UserRoles");

            // 主键
            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.Id).ValueGeneratedOnAdd();

            // 关系配置 - 如果需要配置外键关系的话，取消下面的注释并修改
            // builder.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId);
            // builder.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId);

            // 唯一索引 - 同一用户不能拥有相同的角色两次
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
            
            // 普通索引
            builder.HasIndex(ur => ur.UserId);
            builder.HasIndex(ur => ur.RoleId);
        }
    }
} 