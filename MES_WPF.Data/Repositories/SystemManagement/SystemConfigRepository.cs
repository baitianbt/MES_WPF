using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class SystemConfigRepository : Repository<SystemConfig>, ISystemConfigRepository
    {
        public SystemConfigRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据配置键获取配置
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置对象</returns>
        public async Task<SystemConfig> GetByKeyAsync(string configKey)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sc => sc.ConfigKey == configKey && sc.Status == 1); // 1表示启用状态
        }
        
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <returns>配置值</returns>
        public async Task<string> GetConfigValueAsync(string configKey)
        {
            var config = await GetByKeyAsync(configKey);
            return config?.ConfigValue;
        }
        
        /// <summary>
        /// 批量获取配置值
        /// </summary>
        /// <param name="configKeys">配置键集合</param>
        /// <returns>配置键值对</returns>
        public async Task<IDictionary<string, string>> GetConfigValuesAsync(IEnumerable<string> configKeys)
        {
            var configs = await _dbSet
                .Where(sc => configKeys.Contains(sc.ConfigKey) && sc.Status == 1)
                .ToListAsync();
            
            return configs.ToDictionary(c => c.ConfigKey, c => c.ConfigValue);
        }
        
        /// <summary>
        /// 根据配置类型获取配置
        /// </summary>
        /// <param name="configType">配置类型</param>
        /// <returns>配置列表</returns>
        public async Task<IEnumerable<SystemConfig>> GetByTypeAsync(string configType)
        {
            return await _dbSet
                .Where(sc => sc.ConfigType == configType && sc.Status == 1)
                .OrderBy(sc => sc.ConfigKey)
                .ToListAsync();
        }
        
        /// <summary>
        /// 检查配置键是否已存在
        /// </summary>
        /// <param name="configKey">配置键</param>
        /// <param name="excludeId">排除的配置ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsConfigKeyExistsAsync(string configKey, int? excludeId = null)
        {
            var query = _dbSet.Where(sc => sc.ConfigKey == configKey);
            
            if (excludeId.HasValue)
            {
                query = query.Where(sc => sc.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
} 