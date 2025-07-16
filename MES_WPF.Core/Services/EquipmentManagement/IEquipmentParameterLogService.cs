using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 设备参数记录服务接口
    /// </summary>
    public interface IEquipmentParameterLogService : IService<EquipmentParameterLog>
    {
        /// <summary>
        /// 根据设备ID获取参数记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>参数记录列表</returns>
        Task<IEnumerable<EquipmentParameterLog>> GetByEquipmentIdAsync(int equipmentId);
        
        /// <summary>
        /// 根据参数代码获取参数记录
        /// </summary>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录列表</returns>
        Task<IEnumerable<EquipmentParameterLog>> GetByParameterCodeAsync(string parameterCode);
        
        /// <summary>
        /// 获取报警参数记录
        /// </summary>
        /// <param name="isAlarm">是否报警</param>
        /// <returns>参数记录列表</returns>
        Task<IEnumerable<EquipmentParameterLog>> GetByAlarmStatusAsync(bool isAlarm);
        
        /// <summary>
        /// 获取指定日期范围内的参数记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>参数记录列表</returns>
        Task<IEnumerable<EquipmentParameterLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 获取特定设备特定参数的最新记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <returns>参数记录</returns>
        Task<EquipmentParameterLog> GetLatestParameterAsync(int equipmentId, string parameterCode);
        
        /// <summary>
        /// 获取特定设备特定参数的历史趋势
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <param name="parameterCode">参数代码</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>参数记录列表</returns>
        Task<IEnumerable<EquipmentParameterLog>> GetParameterTrendAsync(int equipmentId, string parameterCode, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 批量添加参数记录
        /// </summary>
        /// <param name="logs">参数记录列表</param>
        /// <returns>添加的参数记录数量</returns>
        Task<int> AddRangeAsync(IEnumerable<EquipmentParameterLog> logs);
    }
}