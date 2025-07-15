using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 角色权限关联表
    /// </summary>
    public class RolePermission
    {
        /// <summary>
        /// 关联唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreateBy { get; set; }
    }
} 