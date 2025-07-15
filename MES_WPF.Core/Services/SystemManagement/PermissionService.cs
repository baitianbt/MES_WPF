using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 权限服务实现
    /// </summary>
    public class PermissionService : Service<Permission>, IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="permissionRepository">权限仓储</param>
        /// <param name="userRepository">用户仓储</param>
        public PermissionService(
            IPermissionRepository permissionRepository,
            IUserRepository userRepository) 
            : base(permissionRepository)
        {
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// 根据权限编码获取权限
        /// </summary>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>权限</returns>
        public async Task<Permission> GetByPermissionCodeAsync(string permissionCode)
        {
            return await _permissionRepository.GetByCodeAsync(permissionCode);
        }
        
        /// <summary>
        /// 获取所有菜单权限
        /// </summary>
        /// <returns>菜单权限列表</returns>
        public async Task<IEnumerable<Permission>> GetMenuPermissionsAsync()
        {
            return await _permissionRepository.GetPermissionsByTypeAsync(1); // 1表示菜单权限
        }
        
        /// <summary>
        /// 获取所有按钮权限
        /// </summary>
        /// <returns>按钮权限列表</returns>
        public async Task<IEnumerable<Permission>> GetButtonPermissionsAsync()
        {
            return await _permissionRepository.GetPermissionsByTypeAsync(2); // 2表示按钮权限
        }
        
        /// <summary>
        /// 获取所有数据权限
        /// </summary>
        /// <returns>数据权限列表</returns>
        public async Task<IEnumerable<Permission>> GetDataPermissionsAsync()
        {
            return await _permissionRepository.GetPermissionsByTypeAsync(3); // 3表示数据权限
        }
        
        /// <summary>
        /// 获取指定父权限的子权限
        /// </summary>
        /// <param name="parentId">父权限ID</param>
        /// <returns>子权限列表</returns>
        public async Task<IEnumerable<Permission>> GetChildPermissionsAsync(int parentId)
        {
            return await _permissionRepository.GetChildrenAsync(parentId);
        }
        
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns>权限树</returns>
        public async Task<IEnumerable<Permission>> GetPermissionTreeAsync()
        {
            // 获取所有权限
            var allPermissions = await GetAllAsync();
            
            // 构建权限树
            return BuildPermissionTree(allPermissions.ToList(), null);
        }
        
        /// <summary>
        /// 获取用户的权限树
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限树</returns>
        public async Task<IEnumerable<Permission>> GetUserPermissionTreeAsync(int userId)
        {
            // 获取用户的所有权限
            var userPermissions = await _userRepository.GetUserPermissionsAsync(userId);
            
            // 构建权限树
            return BuildPermissionTree(userPermissions.ToList(), null);
        }
        
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <param name="permissions">权限列表</param>
        /// <param name="parentId">父权限ID</param>
        /// <returns>权限树</returns>
        private IEnumerable<Permission> BuildPermissionTree(List<Permission> permissions, int? parentId)
        {
            // 获取当前层级的权限
            var nodes = permissions.Where(p => p.ParentId == parentId).ToList();
            
            // 递归构建子节点
            foreach (var node in nodes)
            {
                var children = BuildPermissionTree(permissions, node.Id);
                // 这里我们不能直接设置子节点，因为Permission实体没有Children属性
                // 在实际应用中，可能需要创建一个PermissionTreeNode类来表示树节点
                // 或者在前端构建树结构
            }
            
            return nodes;
        }
    }
} 