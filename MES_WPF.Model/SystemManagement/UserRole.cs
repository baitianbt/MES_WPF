using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 用户角色关联表
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// 关联唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

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