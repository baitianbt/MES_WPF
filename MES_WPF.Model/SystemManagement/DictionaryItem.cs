using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 字典项表
    /// </summary>
    public class DictionaryItem
    {
        /// <summary>
        /// 字典项唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 关联字典ID
        /// </summary>
        public int DictId { get; set; }

        /// <summary>
        /// 字典项值
        /// </summary>
        public string ItemValue { get; set; }

        /// <summary>
        /// 字典项文本
        /// </summary>
        public string ItemText { get; set; }

        /// <summary>
        /// 字典项描述
        /// </summary>
        public string ItemDesc { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 状态(1:启用,2:禁用)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
} 