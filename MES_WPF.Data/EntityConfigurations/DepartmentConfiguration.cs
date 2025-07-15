using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            // 表名
            builder.ToTable("Departments");

            // 主键
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(d => d.DeptCode).IsRequired().HasMaxLength(50);
            builder.Property(d => d.DeptName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.DeptPath).IsRequired().HasMaxLength(500);
            builder.Property(d => d.Leader).HasMaxLength(50);
            builder.Property(d => d.Phone).HasMaxLength(20);
            builder.Property(d => d.Email).HasMaxLength(100);
            builder.Property(d => d.Remark).HasMaxLength(200).IsRequired(false);

            // 自引用关系 - 父子部门
            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(d => d.ParentId)
                .IsRequired(false) // 允许为空，表示顶级部门
                .OnDelete(DeleteBehavior.Restrict); // 防止级联删除

            // 唯一索引
            builder.HasIndex(d => d.DeptCode).IsUnique();
            
            // 普通索引
            builder.HasIndex(d => d.ParentId);
            builder.HasIndex(d => d.Status);
        }
    }
} 