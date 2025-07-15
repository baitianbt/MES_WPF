using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据权限编码获取权限
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>权限对象</returns>
        public async Task<Permission> GetByCodeAsync(string permissionCode)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PermissionCode == permissionCode);
        }
        
        /// <summary>
        /// 获取所有权限，并组织为树形结构
        /// </summary>
        /// <returns>权限树形结构</returns>
        public async Task<IEnumerable<Permission>> GetPermissionTreeAsync()
        {
            // 获取所有权限
            var permissions = await _dbSet
                .Where(p => p.Status == 1) // 1表示启用状态
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            // 找出顶级权限
            var rootPermissions = permissions.Where(p => p.ParentId == null).ToList();
            
            // 递归设置子权限
            // 注意：这里不是真正的树形结构，只是按照层级返回，前端可以用id和parentId组织树
            return rootPermissions;
        }
        
        /// <summary>
        /// 获取指定类型的权限列表
        /// </summary>
        /// <param name="permissionType">权限类型(1:菜单,2:按钮,3:数据)</param>
        /// <returns>权限列表</returns>
        public async Task<IEnumerable<Permission>> GetPermissionsByTypeAsync(byte permissionType)
        {
            return await _dbSet
                .Where(p => p.PermissionType == permissionType && p.Status == 1)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取指定父权限下的所有子权限
        /// </summary>
        /// <param name="parentId">父权限ID</param>
        /// <returns>子权限列表</returns>
        public async Task<IEnumerable<Permission>> GetChildrenAsync(int parentId)
        {
            return await _dbSet
                .Where(p => p.ParentId == parentId && p.Status == 1)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();
        }
    }
} 