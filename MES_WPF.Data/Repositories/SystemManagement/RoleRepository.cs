using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据角色编码获取角色
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <returns>角色对象</returns>
        public async Task<Role> GetByCodeAsync(string roleCode)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RoleCode == roleCode);
        }
        
        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限列表</returns>
        public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.Permissions
                .Join(_context.RolePermissions.Where(rp => rp.RoleId == roleId),
                    permission => permission.Id,
                    rolePermission => rolePermission.PermissionId,
                    (permission, rolePermission) => permission)
                .Where(p => p.Status == 1) // 1表示启用状态
                .ToListAsync();
        }
        
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        public async Task AssignPermissionsAsync(int roleId, IEnumerable<int> permissionIds, int operatorId)
        {
            // 开启事务
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 删除角色原有权限
                var existingRolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();
                
                _context.RolePermissions.RemoveRange(existingRolePermissions);
                
                // 添加新的权限关联
                if (permissionIds != null && permissionIds.Any())
                {
                    var rolePermissions = permissionIds.Select(pid => new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = pid,
                        CreateBy = operatorId,
                        CreateTime = DateTime.Now
                    });
                    
                    await _context.RolePermissions.AddRangeAsync(rolePermissions);
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        /// <summary>
        /// 获取角色的用户列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>用户列表</returns>
        public async Task<IEnumerable<User>> GetRoleUsersAsync(int roleId)
        {
            return await _context.Users
                .Join(_context.UserRoles.Where(ur => ur.RoleId == roleId),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => user)
                .Where(u => u.Status == 1) // 1表示正常状态
                .ToListAsync();
        }
    }
} 