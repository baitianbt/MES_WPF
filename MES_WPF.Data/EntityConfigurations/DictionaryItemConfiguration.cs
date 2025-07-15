using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES_WPF.Core.Models;

namespace MES_WPF.Data.EntityConfigurations
{
    public class DictionaryItemConfiguration : IEntityTypeConfiguration<DictionaryItem>
    {
        public void Configure(EntityTypeBuilder<DictionaryItem> builder)
        {
            // 表名
            builder.ToTable("DictionaryItems");

            // 主键
            builder.HasKey(di => di.Id);
            builder.Property(di => di.Id).ValueGeneratedOnAdd();

            // 属性配置
            builder.Property(di => di.ItemValue).IsRequired().HasMaxLength(100);
            builder.Property(di => di.ItemText).IsRequired().HasMaxLength(100);
            builder.Property(di => di.ItemDesc).HasMaxLength(200);
            builder.Property(di => di.Remark).HasMaxLength(200);

            // 关系配置
            // builder.HasOne<Dictionary>().WithMany().HasForeignKey(di => di.DictId);

            // 唯一索引 - 确保同一字典下的字典项值不重复
            builder.HasIndex(di => new { di.DictId, di.ItemValue }).IsUnique();
            
            // 普通索引
            builder.HasIndex(di => di.DictId);
            builder.HasIndex(di => di.ItemValue);
            builder.HasIndex(di => di.Status);
        }
    }
} 