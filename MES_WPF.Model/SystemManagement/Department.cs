using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 部门表
    /// </summary>
    public class Department
    {
        /// <summary>
        /// 部门唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 父部门ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 部门路径
        /// </summary>
        public string DeptPath { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Leader { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 状态(1:正常,2:禁用)
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