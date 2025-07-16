

using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserService : IService<User>
    {
        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户</returns>
        Task<User> GetByUsernameAsync(string username);
        
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="clientIP">客户端IP</param>
        /// <returns>登录成功返回用户信息，失败返回null</returns>
        Task<User> LoginAsync(string username, string password, string clientIP);
        
        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色列表</returns>
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        
        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限列表</returns>
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);
        
        /// <summary>
        /// 检查用户是否有指定权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>是否有权限</returns>
        Task<bool> HasPermissionAsync(int userId, string permissionCode);
        
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>是否成功</returns>
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> ResetPasswordAsync(int userId, string newPassword, int operatorId);
        
        /// <summary>
        /// 分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> AssignRolesAsync(int userId, IEnumerable<int> roleIds, int operatorId);
    }
} 