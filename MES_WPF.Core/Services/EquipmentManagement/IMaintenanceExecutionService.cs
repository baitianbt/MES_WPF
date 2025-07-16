using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 维护执行记录服务接口
    /// </summary>
    public interface IMaintenanceExecutionService : IService<MaintenanceExecution>
    {
        /// <summary>
        /// 根据维护工单ID获取维护执行记录
        /// </summary>
        /// <param name="maintenanceOrderId">维护工单ID</param>
        /// <returns>维护执行记录列表</returns>
        Task<IEnumerable<MaintenanceExecution>> GetByMaintenanceOrderIdAsync(int maintenanceOrderId);
        
        /// <summary>
        /// 根据执行人ID获取维护执行记录
        /// </summary>
        /// <param name="executorId">执行人ID</param>
        /// <returns>维护执行记录列表</returns>
        Task<IEnumerable<MaintenanceExecution>> GetByExecutorIdAsync(int executorId);
        
        /// <summary>
        /// 根据执行结果获取维护执行记录
        /// </summary>
        /// <param name="executionResult">执行结果</param>
        /// <returns>维护执行记录列表</returns>
        Task<IEnumerable<MaintenanceExecution>> GetByExecutionResultAsync(byte executionResult);
        
        /// <summary>
        /// 获取指定日期范围内的维护执行记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护执行记录列表</returns>
        Task<IEnumerable<MaintenanceExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 获取指定设备的维护执行记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护执行记录列表</returns>
        Task<IEnumerable<MaintenanceExecution>> GetByEquipmentIdAsync(int equipmentId);
        
        /// <summary>
        /// 结束维护执行记录
        /// </summary>
        /// <param name="id">执行记录ID</param>
        /// <param name="executionResult">执行结果</param>
        /// <param name="resultDescription">结果描述</param>
        /// <returns>更新后的执行记录</returns>
        Task<MaintenanceExecution> CompleteExecutionAsync(int id, byte executionResult, string resultDescription);
    }
}