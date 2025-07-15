using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;



namespace MES_WPF.Data.EntityConfigurations
{
    public class DictionaryConfiguration : IEntityTypeConfiguration<Dictionary>
    {
        public void Configure(EntityTypeBuilder<Dictionary> builder)
        {
            // 表名
            builder.ToTable("Dictionaries");

            // 主键
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(d => d.DictType).IsRequired().HasMaxLength(100);
            builder.Property(d => d.DictName).IsRequired().HasMaxLength(100);
            builder.Property(d => d.Remark).HasMaxLength(200);

            // 唯一索引
            builder.HasIndex(d => d.DictType).IsUnique();
            
            // 普通索引
            builder.HasIndex(d => d.Status);
        }
    }
} 