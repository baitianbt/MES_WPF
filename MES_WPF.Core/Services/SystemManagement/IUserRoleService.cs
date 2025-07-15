using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 用户角色关联服务接口
    /// </summary>
    public interface IUserRoleService : IService<UserRole>
    {
        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID列表</param>
        /// <param name="createBy">创建人ID</param>
        /// <returns>任务</returns>
        Task AssignRolesAsync(int userId, IEnumerable<int> roleIds, int createBy);
        
        /// <summary>
        /// 获取用户拥有的角色ID列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色ID列表</returns>
        Task<List<int>> GetRoleIdsByUserIdAsync(int userId);
        
        /// <summary>
        /// 检查用户是否拥有指定角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns>是否拥有</returns>
        Task<bool> HasRoleAsync(int userId, int roleId);
    }
} 