using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IPermissionRepository : IRepository<Permission>
    {
        /// <summary>
        /// 根据权限编码获取权限
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>权限对象</returns>
        Task<Permission> GetByCodeAsync(string permissionCode);
        
        /// <summary>
        /// 获取所有权限，并组织为树形结构
        /// </summary>
        /// <returns>权限树形结构</returns>
        Task<IEnumerable<Permission>> GetPermissionTreeAsync();
        
        /// <summary>
        /// 获取指定类型的权限列表
        /// </summary>
        /// <param name="permissionType">权限类型(1:菜单,2:按钮,3:数据)</param>
        /// <returns>权限列表</returns>
        Task<IEnumerable<Permission>> GetPermissionsByTypeAsync(byte permissionType);
        
        /// <summary>
        /// 获取指定父权限下的所有子权限
        /// </summary>
        /// <param name="parentId">父权限ID</param>
        /// <returns>子权限列表</returns>
        Task<IEnumerable<Permission>> GetChildrenAsync(int parentId);
    }
} 