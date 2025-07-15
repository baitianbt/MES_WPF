using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // 表名
            builder.ToTable("Employees");

            // 主键
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
            builder.Property(e => e.EmployeeName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.IdCard).HasMaxLength(20);
            builder.Property(e => e.Phone).HasMaxLength(20);
            builder.Property(e => e.Email).HasMaxLength(100);
            builder.Property(e => e.Position).HasMaxLength(50);
            builder.Property(e => e.Remark).HasMaxLength(200);

            // 关系配置
            // builder.HasOne<Department>().WithMany().HasForeignKey(e => e.DeptId);

            // 唯一索引
            builder.HasIndex(e => e.EmployeeCode).IsUnique();
            
            // 普通索引
            builder.HasIndex(e => e.EmployeeName);
            builder.HasIndex(e => e.DeptId);
            builder.HasIndex(e => e.Status);
        }
    }
} 