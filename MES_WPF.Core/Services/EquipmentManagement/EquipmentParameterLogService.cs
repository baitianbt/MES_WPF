using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 设备参数记录服务实现
    /// </summary>
    public class EquipmentParameterLogService : Service<EquipmentParameterLog>, IEquipmentParameterLogService
    {
        private readonly IEquipmentParameterLogRepository _equipmentParameterLogRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">设备参数记录仓储</param>
        public EquipmentParameterLogService(IEquipmentParameterLogRepository repository) : base(repository)
        {
            _equipmentParameterLogRepository = repository;
        }
        
        /// <summary>
        /// 根据设备ID获取参数记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _equipmentParameterLogRepository.GetByEquipmentIdAsync(equipmentId);
        }
        
        /// <summary>
        /// 根据参数代码获取参数记录
        /// </summary>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByParameterCodeAsync(string parameterCode)
        {
            return await _equipmentParameterLogRepository.GetByParameterCodeAsync(parameterCode);
        }
        
        /// <summary>
        /// 获取报警参数记录
        /// </summary>
        /// <param name="isAlarm">是否报警</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByAlarmStatusAsync(bool isAlarm)
        {
            return await _equipmentParameterLogRepository.GetByAlarmStatusAsync(isAlarm);
        }
        
        /// <summary>
        /// 获取指定日期范围内的参数记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>参数记录列表</returns>
        public async Task<IEnumerable<EquipmentParameterLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _equipmentParameterLogRepository.GetByDateRangeAsync(startDate, endDate);
        }
        
        /// <summary>
        /// 获取特定设备特定参数的最新记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录</returns>
        public async Task<EquipmentParameterLog> GetLatestParameterAsync(int equipmentId, string parameterCode)
        {
            return await _equipmentParameterLogRepository.GetLatestParameterAsync(equipmentId, parameterCode);
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
            return await _equipmentParameterLogRepository.GetParameterTrendAsync(equipmentId, parameterCode, startDate, endDate);
        }
        
        /// <summary>
        /// 批量添加参数记录
        /// </summary>
        /// <param name="logs">参数记录列表</param>
        /// <returns>添加的参数记录数量</returns>
        public async Task<int> AddRangeAsync(IEnumerable<EquipmentParameterLog> logs)
        {
            return await _equipmentParameterLogRepository.AddRangeAsync(logs);
        }
    }
} 