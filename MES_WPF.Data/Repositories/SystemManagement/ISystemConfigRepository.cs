using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface ISystemConfigRepository : IRepository<SystemConfig>
    {
        /// <summary>
        /// 根据配置键获取配置
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置对象</returns>
        Task<SystemConfig> GetByKeyAsync(string configKey);
        
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置值</returns>
        Task<string> GetConfigValueAsync(string configKey);
        
        /// <summary>
        /// 批量获取配置值
        /// </summary>
        /// <param name="configKeys">配置键集合</param>
        /// <returns>配置键值对</returns>
        Task<IDictionary<string, string>> GetConfigValuesAsync(IEnumerable<string> configKeys);
        
        /// <summary>
        /// 根据配置类型获取配置
        /// </summary>
        /// <param name="configType">配置类型</param>
        /// <returns>配置列表</returns>
        Task<IEnumerable<SystemConfig>> GetByTypeAsync(string configType);
        
        /// <summary>
        /// 检查配置键是否已存在
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <param name="excludeId">排除的配置ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsConfigKeyExistsAsync(string configKey, int? excludeId = null);
    }
} 