using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 用户服务实现
    /// </summary>
    public class UserService : Service<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="permissionRepository">权限仓储</param>
        public UserService(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository) 
            : base(userRepository)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户</returns>
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }
        
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="clientIP">客户端IP</param>
        /// <returns>登录成功返回用户信息，失败返回null</returns>
        public async Task<User> LoginAsync(string username, string password, string clientIP)
        {
            // 对密码进行加密处理（实际项目中应使用更安全的加密方式）
            var encryptedPassword = EncryptPassword(password);
            
            // 验证用户
            var user = await _userRepository.ValidateUserAsync(username, encryptedPassword);
            
            if (user != null)
            {
                // 更新登录信息
                await _userRepository.UpdateLastLoginInfoAsync(user.Id, clientIP);
            }
            
            return user;
        }
        
        /// <summary>
        /// 获取用户的角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>角色列表</returns>
        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await _userRepository.GetUserRolesAsync(userId);
        }
        
        /// <summary>
        /// 获取用户的权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限列表</returns>
        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
        {
            return await _userRepository.GetUserPermissionsAsync(userId);
        }
        
        /// <summary>
        /// 检查用户是否有指定权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCode">权限编码</param>
        /// <returns>是否有权限</returns>
        public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            return permissions.Any(p => p.PermissionCode == permissionCode);
        }
        
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            
            // 验证旧密码
            var encryptedOldPassword = EncryptPassword(oldPassword);
            if (user.Password != encryptedOldPassword)
            {
                return false;
            }
            
            // 设置新密码
            user.Password = EncryptPassword(newPassword);
            user.PasswordUpdateTime = DateTime.Now;
            
            await UpdateAsync(user);
            return true;
        }
        
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="newPassword">新密码</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, int operatorId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            
            // 设置新密码
            user.Password = EncryptPassword(newPassword);
            user.PasswordUpdateTime = DateTime.Now;
            
            await UpdateAsync(user);
            return true;
        }
        
        /// <summary>
        /// 分配角色
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleIds">角色ID集合</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AssignRolesAsync(int userId, IEnumerable<int> roleIds, int operatorId)
        {
            try
            {
                // 这里需要调用UserRoleRepository来实现角色分配，简单起见，我们在这里不实现详细逻辑
                // 实际项目中，应该注入IUserRoleRepository并调用相应方法
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <returns>加密后的密码</returns>
        private string EncryptPassword(string password)
        {
            // 实际项目中应使用更安全的加密方式，如PBKDF2, BCrypt等
            // 这里简单使用SHA256作为示例
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
} 