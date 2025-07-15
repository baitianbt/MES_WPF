using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>角色对象</returns>
        Task<Role> GetByCodeAsync(string roleCode);
        
        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId);
        
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int operatorId);
        
        /// <summary>
        /// 获取角色的用户列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>用户列表</returns>
        Task<IEnumerable<User>> GetRoleUsersAsync(int roleId);
    }
} 