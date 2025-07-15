using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
    {
        public void Configure(EntityTypeBuilder<SystemConfig> builder)
        {
            // 表名
            builder.ToTable("SystemConfigs");

            // 主键
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(sc => sc.ConfigKey).IsRequired().HasMaxLength(100);
            builder.Property(sc => sc.ConfigValue).IsRequired().HasMaxLength(500);
            builder.Property(sc => sc.ConfigName).IsRequired().HasMaxLength(100);
            builder.Property(sc => sc.ConfigType).IsRequired().HasMaxLength(50);
            builder.Property(sc => sc.Remark).HasMaxLength(200);

            // 唯一索引
            builder.HasIndex(sc => sc.ConfigKey).IsUnique();
            
            // 普通索引
            builder.HasIndex(sc => sc.ConfigType);
            builder.HasIndex(sc => sc.Status);
        }
    }
} 