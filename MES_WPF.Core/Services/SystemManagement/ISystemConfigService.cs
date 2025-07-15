using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 系统配置服务接口
    /// </summary>
    public interface ISystemConfigService : IService<SystemConfig>
    {
        /// <summary>
        /// 根据配置键获取配置
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置</returns>
        Task<SystemConfig> GetByConfigKeyAsync(string configKey);
        
        /// <summary>
        /// 根据配置键获取配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置值</returns>
        Task<string> GetConfigValueAsync(string configKey);
        
        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <param name="configValue">配置值</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> SetConfigValueAsync(string configKey, string configValue, int operatorId);
        
        /// <summary>
        /// 获取系统配置列表
        /// </summary>
        /// <returns>系统配置列表</returns>
        Task<IEnumerable<SystemConfig>> GetSystemConfigsAsync();
        
        /// <summary>
        /// 获取用户配置列表
        /// </summary>
        /// <returns>用户配置列表</returns>
        Task<IEnumerable<SystemConfig>> GetUserConfigsAsync();
        
        /// <summary>
        /// 根据配置类型获取配置列表
        /// </summary>
        /// <param name="configType">配置类型</param>
        /// <returns>配置列表</returns>
        Task<IEnumerable<SystemConfig>> GetByConfigTypeAsync(string configType);
        
        /// <summary>
        /// 批量更新配置
        /// </summary>
        /// <param name="configs">配置列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        Task<bool> BatchUpdateConfigsAsync(IEnumerable<SystemConfig> configs, int operatorId);
    }
} 