using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 数据字典表
    /// </summary>
    public class Dictionary
    {
        /// <summary>
        /// 字典唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        public string DictName { get; set; }

        /// <summary>
        /// 状态(1:启用,2:禁用)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateBy { get; set; }

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