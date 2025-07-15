using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    /// <summary>
    /// 用户角色关联表仓储接口
    /// </summary>
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        /// <summary>
        /// 根据用户ID获取角色ID列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色ID列表</returns>
        Task<List<int>> GetRoleIdsByUserIdAsync(int userId);
        
        /// <summary>
        /// 根据用户ID删除所有关联记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>任务</returns>
        Task DeleteByUserIdAsync(int userId);
    }
} 