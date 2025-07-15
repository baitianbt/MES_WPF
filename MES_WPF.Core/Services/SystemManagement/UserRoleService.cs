using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 用户角色关联服务实现
    /// </summary>
    public class UserRoleService : Service<UserRole>, IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRoleRepository">用户角色仓储</param>
        public UserRoleService(IUserRoleRepository userRoleRepository) 
            : base(userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }
        
        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID列表</param>
        /// <param name="createBy">创建人ID</param>
        /// <returns>任务</returns>
        public async Task AssignRolesAsync(int userId, IEnumerable<int> roleIds, int createBy)
        {
            // 首先删除用户所有角色关联
            await _userRoleRepository.DeleteByUserIdAsync(userId);
            
            // 添加新角色关联
            foreach (var roleId in roleIds)
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreateTime = DateTime.Now,
                    CreateBy = createBy
                };
                
                await _userRoleRepository.AddAsync(userRole);
            }
        }
        
        /// <summary>
        /// 获取用户拥有的角色ID列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色ID列表</returns>
        public async Task<List<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            return await _userRoleRepository.GetRoleIdsByUserIdAsync(userId);
        }
        
        /// <summary>
        /// 检查用户是否拥有指定角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>是否拥有</returns>
        public async Task<bool> HasRoleAsync(int userId, int roleId)
        {
            var roles = await GetRoleIdsByUserIdAsync(userId);
            return roles.Contains(roleId);
        }
    }
} 