using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 角色权限关联服务实现
    /// </summary>
    public class RolePermissionService : Service<RolePermission>, IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rolePermissionRepository">角色权限仓储</param>
        public RolePermissionService(IRolePermissionRepository rolePermissionRepository) 
            : base(rolePermissionRepository)
        {
            _rolePermissionRepository = rolePermissionRepository;
        }
        
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID列表</param>
        /// <param name="createBy">创建人ID</param>
        /// <returns>任务</returns>
        public async Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int createBy)
        {
            // 首先查找该角色所有已有的权限关联
            var existingPermissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId);
            var existingPermissionIds = existingPermissions.Select(rp => rp.PermissionId);
            
            // 需要添加的权限ID
            var permissionsToAdd = permissionIds.Except(existingPermissionIds);
            
            // 需要删除的权限关联
            var permissionsToRemove = existingPermissions.Where(rp => !permissionIds.Contains(rp.PermissionId));
            
            // 添加新权限关联
            foreach (var permissionId in permissionsToAdd)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    CreateTime = DateTime.Now,
                    CreateBy = createBy
                };
                
                await _rolePermissionRepository.AddAsync(rolePermission);
            }
            
            // 删除不再需要的权限关联
            foreach (var permission in permissionsToRemove)
            {
                await _rolePermissionRepository.DeleteAsync(permission);
            }
        }
        
        /// <summary>
        /// 获取角色拥有的权限ID列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限ID列表</returns>
        public async Task<IEnumerable<int>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var permissions = await _rolePermissionRepository.FindAsync(rp => rp.RoleId == roleId);
            return permissions.Select(rp => rp.PermissionId);
        }
    }
} 