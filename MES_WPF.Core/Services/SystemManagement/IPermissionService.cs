using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 权限服务接口
    /// </summary>
    public interface IPermissionService : IService<Permission>
    {
        /// <summary>
        /// 根据权限编码获取权限
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>权限</returns>
        Task<Permission> GetByPermissionCodeAsync(string permissionCode);
        
        /// <summary>
        /// 获取所有菜单权限
        /// </summary>
        /// <returns>菜单权限列表</returns>
        Task<IEnumerable<Permission>> GetMenuPermissionsAsync();
        
        /// <summary>
        /// 获取所有按钮权限
        /// </summary>
        /// <returns>按钮权限列表</returns>
        Task<IEnumerable<Permission>> GetButtonPermissionsAsync();
        
        /// <summary>
        /// 获取所有数据权限
        /// </summary>
        /// <returns>数据权限列表</returns>
        Task<IEnumerable<Permission>> GetDataPermissionsAsync();
        
        /// <summary>
        /// 获取指定父权限的子权限
        /// </summary>
        /// <param name="parentId">父权限ID</param>
        /// <returns>子权限列表</returns>
        Task<IEnumerable<Permission>> GetChildPermissionsAsync(int parentId);
        
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns>权限树</returns>
        Task<IEnumerable<Permission>> GetPermissionTreeAsync();
        
        /// <summary>
        /// 获取用户的权限树
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限树</returns>
        Task<IEnumerable<Permission>> GetUserPermissionTreeAsync(int userId);
    }
} 