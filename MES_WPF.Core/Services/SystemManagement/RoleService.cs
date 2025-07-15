using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 角色服务实现
    /// </summary>
    public class RoleService : Service<Role>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="roleRepository">角色仓储</param>
        /// <param name="rolePermissionRepository">角色权限仓储</param>
        /// <param name="permissionRepository">权限仓储</param>
        public RoleService(
            IRoleRepository roleRepository,
            IRolePermissionRepository rolePermissionRepository,
            IPermissionRepository permissionRepository) 
            : base(roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _rolePermissionRepository = rolePermissionRepository ?? throw new ArgumentNullException(nameof(rolePermissionRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        }

        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>角色</returns>
        public async Task<Role> GetByRoleCodeAsync(string roleCode)
        {
            return await _roleRepository.GetByCodeAsync(roleCode);
        }
        
        /// <summary>
        /// 获取角色的权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _roleRepository.GetRolePermissionsAsync(roleId);
        }
        
        /// <summary>
        /// 检查角色是否有指定权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>是否有权限</returns>
        public async Task<bool> HasPermissionAsync(int roleId, string permissionCode)
        {
            var permissions = await GetRolePermissionsAsync(roleId);
            return permissions.Any(p => p.PermissionCode == permissionCode);
        }
        
        /// <summary>
        /// 分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int operatorId)
        {
            try
            {
                // 先删除角色原有的权限
                await _rolePermissionRepository.DeleteByIdAsync(roleId);
                
                // 添加新的权限
                foreach (var permissionId in permissionIds)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        CreateBy = operatorId,
                        CreateTime = DateTime.Now
                    };
                    
                    await _rolePermissionRepository.AddAsync(rolePermission);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取所有系统角色
        /// </summary>
        /// <returns>系统角色列表</returns>
        public async Task<IEnumerable<Role>> GetSystemRolesAsync()
        {
            throw new NotImplementedException("Method not implemented yet.");

            //return await _roleRepository.GetRolesByTypeAsync(1); // 1表示系统角色
        }
        
        /// <summary>
        /// 获取所有业务角色
        /// </summary>
        /// <returns>业务角色列表</returns>
        public async Task<IEnumerable<Role>> GetBusinessRolesAsync()
        {
            throw new NotImplementedException("Method not implemented yet.");

            //return await _roleRepository.GetRolesByTypeAsync(2); // 2表示业务角色
        }
    }
} 