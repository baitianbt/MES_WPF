using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// 根据用户名查找用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户对象</returns>
        Task<User> GetByUsernameAsync(string username);

        /// <summary>
        /// 验证用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>用户对象</returns>
        Task<User> ValidateUserAsync(string username, string password);

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色列表</returns>
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);

        /// <summary>
        /// 获取用户拥有的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限列表</returns>
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);

        /// <summary>
        /// 更新用户最后登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="loginIp">登录IP</param>
        /// <returns>操作结果</returns>
        Task UpdateLastLoginInfoAsync(int userId, string loginIp);
    }
} 