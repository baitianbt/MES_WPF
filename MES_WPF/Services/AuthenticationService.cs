using MES_WPF.Core.Models;
using MES_WPF.Core.Services.SystemManagement;
using System;
using System.Threading.Tasks;

namespace MES_WPF.Services
{
    /// <summary>
    /// 认证服务实现
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        
        /// <summary>
        /// 是否已认证
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        public int CurrentUserId { get; private set; }

        /// <summary>
        /// 当前用户名
        /// </summary>
        public string CurrentUsername { get; private set; } = string.Empty;
        
        /// <summary>
        /// 当前用户
        /// </summary>
        public User? CurrentUser { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userService">用户服务</param>
        public AuthenticationService(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录是否成功</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                // 获取客户端IP (实际项目中可能需要通过其他方式获取)
                string clientIp = "127.0.0.1";
                
                // 调用用户服务进行登录验证
                var user = await _userService.LoginAsync(username, password, clientIp);
                
                if (user != null)
                {
                    IsAuthenticated = true;
                    CurrentUserId = user.Id;
                    CurrentUsername = user.Username;
                    CurrentUser = user;
                    return true;
                }
                
                return false;
            }
            catch (Exception)
            {
                // 登录失败
                return false;
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Logout()
        {
            IsAuthenticated = false;
            CurrentUserId = 0;
            CurrentUsername = string.Empty;
            CurrentUser = null;
        }
        
        /// <summary>
        /// 异步注销
        /// </summary>
        public async Task LogoutAsync()
        {
            // 执行一些异步清理操作
            await Task.Run(() => Logout());
        }
    }
}