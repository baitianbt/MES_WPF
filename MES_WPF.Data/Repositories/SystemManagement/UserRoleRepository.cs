using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    /// <summary>
    /// 用户角色关联表仓储实现
    /// </summary>
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public UserRoleRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 根据用户ID获取角色ID列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色ID列表</returns>
        public async Task<List<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();
        }
        
        /// <summary>
        /// 根据用户ID删除所有关联记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>任务</returns>
        public async Task DeleteByUserIdAsync(int userId)
        {
            var userRoles = await _dbSet.Where(ur => ur.UserId == userId).ToListAsync();
            if (userRoles.Any())
            {
                _dbSet.RemoveRange(userRoles);
                await SaveChangesAsync();
            }
        }
    }
} 