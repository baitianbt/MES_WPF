using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户对象</returns>
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// 验证用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户对象</returns>
        public async Task<User> ValidateUserAsync(string username, string password)
        {
            // 注意：实际应用中应该对密码进行哈希比较
            return await _dbSet.FirstOrDefaultAsync(u => 
                u.Username == username && 
                u.Password == password && 
                u.Status == 1); // 1表示正常状态
        }

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色列表</returns>
        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _context.Roles
                .Join(_context.UserRoles.Where(ur => ur.UserId == userId),
                    role => role.Id,
                    userRole => userRole.RoleId,
                    (role, userRole) => role)
                .Where(r => r.Status == 1) // 1表示启用状态
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户拥有的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限列表</returns>
        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
        {
            // 获取用户所有角色的ID
            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // 获取这些角色所拥有的权限
            return await _context.Permissions
                .Join(_context.RolePermissions.Where(rp => roleIds.Contains(rp.RoleId)),
                    permission => permission.Id,
                    rolePermission => rolePermission.PermissionId,
                    (permission, rolePermission) => permission)
                .Where(p => p.Status == 1) // 1表示启用状态
                .Distinct() // 移除重复的权限
                .ToListAsync();
        }

        /// <summary>
        /// 更新用户最后登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loginIp">登录IP</param>
        /// <returns>操作结果</returns>
        public async Task UpdateLastLoginInfoAsync(int userId, string loginIp)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginTime = DateTime.Now;
                user.LastLoginIp = loginIp;
                await SaveChangesAsync();
            }
        }
    }
} 