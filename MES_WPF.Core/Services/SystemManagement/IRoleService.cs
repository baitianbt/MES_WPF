using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 角色服务接口
    /// </summary>
    public interface IRoleService : IService<Role>
    {
        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>角色</returns>
        Task<Role> GetByRoleCodeAsync(string roleCode);
        
        /// <summary>
        /// 获取角色的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId);
        
        /// <summary>
        /// 检查角色是否有指定权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>是否有权限</returns>
        Task<bool> HasPermissionAsync(int roleId, string permissionCode);
        
        /// <summary>
        /// 分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int operatorId);
        
        /// <summary>
        /// 获取所有系统角色
        /// </summary>
        /// <returns>系统角色列表</returns>
        Task<IEnumerable<Role>> GetSystemRolesAsync();
        
        /// <summary>
        /// 获取所有业务角色
        /// </summary>
        /// <returns>业务角色列表</returns>
        Task<IEnumerable<Role>> GetBusinessRolesAsync();
    }
} 