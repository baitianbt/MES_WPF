using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 权限表
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// 权限唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限类型(1:菜单,2:按钮,3:数据)
        /// </summary>
        public byte PermissionType { get; set; }

        /// <summary>
        /// 父权限ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 路径(菜单类型)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 组件(菜单类型)
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 图标(菜单类型)
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

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