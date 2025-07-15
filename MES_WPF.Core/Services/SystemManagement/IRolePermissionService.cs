using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 角色权限关联服务接口
    /// </summary>
    public interface IRolePermissionService : IService<RolePermission>
    {
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID列表</param>
        /// <param name="createBy">创建人ID</param>
        /// <returns>任务</returns>
        Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int createBy);
        
        /// <summary>
        /// 获取角色拥有的权限ID列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限ID列表</returns>
        Task<IEnumerable<int>> GetPermissionsByRoleIdAsync(int roleId);
    }
} 