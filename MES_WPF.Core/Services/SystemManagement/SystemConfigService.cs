using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 系统配置服务实现
    /// </summary>
    public class SystemConfigService : Service<SystemConfig>, ISystemConfigService
    {
        private readonly ISystemConfigRepository _systemConfigRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="systemConfigRepository">系统配置仓储</param>
        public SystemConfigService(ISystemConfigRepository systemConfigRepository) 
            : base(systemConfigRepository)
        {
            _systemConfigRepository = systemConfigRepository ?? throw new ArgumentNullException(nameof(systemConfigRepository));
        }

        /// <summary>
        /// 根据配置键获取配置
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置</returns>
        public async Task<SystemConfig> GetByConfigKeyAsync(string configKey)
        {
            return await _systemConfigRepository.GetByKeyAsync(configKey);
        }
        
        /// <summary>
        /// 根据配置键获取配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置值</returns>
        public async Task<string> GetConfigValueAsync(string configKey)
        {
            var config = await GetByConfigKeyAsync(configKey);
            return config?.ConfigValue;
        }
        
        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <param name="configValue">配置值</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> SetConfigValueAsync(string configKey, string configValue, int operatorId)
        {
            try
            {
                var config = await GetByConfigKeyAsync(configKey);
                if (config == null)
                {
                    return false;
                }
                
                config.ConfigValue = configValue;
                config.UpdateTime = DateTime.Now;
                
                await UpdateAsync(config);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取系统配置列表
        /// </summary>
        /// <returns>系统配置列表</returns>
        public async Task<IEnumerable<SystemConfig>> GetSystemConfigsAsync()
        {
            throw new NotImplementedException("Method not implemented yet.");
            //return await _systemConfigRepository.GetSystemConfigsAsync();
        }

        /// <summary>
        /// 获取用户配置列表
        /// </summary>
        /// <returns>用户配置列表</returns>
        public async Task<IEnumerable<SystemConfig>> GetUserConfigsAsync()
        {
            throw new NotImplementedException("Method not implemented yet.");

            //return await _systemConfigRepository.GetUserConfigsAsync();
        }
        
        /// <summary>
        /// 根据配置类型获取配置列表
        /// </summary>
        /// <param name="configType">配置类型</param>
        /// <returns>配置列表</returns>
        public async Task<IEnumerable<SystemConfig>> GetByConfigTypeAsync(string configType)
        {
            throw new NotImplementedException("Method not implemented yet.");

            //return await _systemConfigRepository.GetByConfigTypeAsync(configType);
        }
        
        /// <summary>
        /// 批量更新配置
        /// </summary>
        /// <param name="configs">配置列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> BatchUpdateConfigsAsync(IEnumerable<SystemConfig> configs, int operatorId)
        {
            try
            {
                foreach (var config in configs)
                {
                    config.UpdateTime = DateTime.Now;
                    await UpdateAsync(config);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 