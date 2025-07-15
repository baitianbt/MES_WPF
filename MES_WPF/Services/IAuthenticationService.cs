using MES_WPF.Core.Models;
using System.Threading.Tasks;

namespace MES_WPF.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 是否已认证
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 当前用户ID
        /// </summary>
        int CurrentUserId { get; }

        /// <summary>
        /// 当前用户名
        /// </summary>
        string CurrentUsername { get; }
        
        /// <summary>
        /// 当前用户
        /// </summary>
        User? CurrentUser { get; }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>登录是否成功</returns>
        Task<bool> LoginAsync(string username, string password);

        /// <summary>
        /// 注销
        /// </summary>
        void Logout();
        
        /// <summary>
        /// 异步注销
        /// </summary>
        Task LogoutAsync();
    }
} 