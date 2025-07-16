using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class EquipmentParameterLogRepository : Repository<EquipmentParameterLog>, IEquipmentParameterLogRepository
    {
        public EquipmentParameterLogRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 根据设备ID获取参数记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _dbSet.Where(log => log.EquipmentId == equipmentId).ToListAsync();
        }
        
        /// <summary>
        /// 根据参数代码获取参数记录
        /// </summary>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByParameterCodeAsync(string parameterCode)
        {
            return await _dbSet.Where(log => log.ParameterCode == parameterCode).ToListAsync();
        }
        
        /// <summary>
        /// 获取报警参数记录
        /// </summary>
        /// <param name="isAlarm">是否报警</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByAlarmStatusAsync(bool isAlarm)
        {
            return await _dbSet.Where(log => log.IsAlarm == isAlarm).ToListAsync();
        }
        
        /// <summary>
        /// 获取指定日期范围内的参数记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(log => log.CollectTime >= startDate && log.CollectTime <= endDate)
                .OrderBy(log => log.CollectTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取特定设备特定参数的最新记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录</returns>
        public async Task<EquipmentParameterLog> GetLatestParameterAsync(int equipmentId, string parameterCode)
        {
            return await _dbSet
                .Where(log => log.EquipmentId == equipmentId && log.ParameterCode == parameterCode)
                .OrderByDescending(log => log.CollectTime)
                .FirstOrDefaultAsync();
        }
        
        /// <summary>
        /// 获取特定设备特定参数的历史趋势
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetParameterTrendAsync(int equipmentId, string parameterCode, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(log => log.EquipmentId == equipmentId && 
                       log.ParameterCode == parameterCode &&
                       log.CollectTime >= startDate && 
                       log.CollectTime <= endDate)
                .OrderBy(log => log.CollectTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 批量添加参数记录
        /// </summary>
        /// <param name="logs">参数记录列表</param>
        /// <returns>添加的参数记录数量</returns>
        public async Task<int> AddRangeAsync(IEnumerable<EquipmentParameterLog> logs)
        {
            await _dbSet.AddRangeAsync(logs);
            return await _context.SaveChangesAsync();
        }
    }
}