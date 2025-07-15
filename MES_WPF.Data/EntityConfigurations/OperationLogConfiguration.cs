using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class OperationLogConfiguration : IEntityTypeConfiguration<OperationLog>
    {
        public void Configure(EntityTypeBuilder<OperationLog> builder)
        {
            // 表名
            builder.ToTable("OperationLogs");

            // 主键
            builder.HasKey(ol => ol.Id);
            builder.Property(ol => ol.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(ol => ol.ModuleType).IsRequired().HasMaxLength(50);
            builder.Property(ol => ol.OperationType).IsRequired().HasMaxLength(50);
            builder.Property(ol => ol.OperationDesc).IsRequired().HasMaxLength(200);
            builder.Property(ol => ol.RequestMethod).HasMaxLength(10);
            builder.Property(ol => ol.RequestUrl).HasMaxLength(200);
            builder.Property(ol => ol.RequestParams).HasMaxLength(2000);
            builder.Property(ol => ol.ResponseResult).HasMaxLength(2000);
            builder.Property(ol => ol.OperationIp).HasMaxLength(50);
            builder.Property(ol => ol.ErrorMsg).HasMaxLength(2000);

            // 普通索引
            builder.HasIndex(ol => ol.ModuleType);
            builder.HasIndex(ol => ol.OperationType);
            builder.HasIndex(ol => ol.OperationTime);
            builder.HasIndex(ol => ol.OperationUser);
            builder.HasIndex(ol => ol.Status);
        }
    }
} 