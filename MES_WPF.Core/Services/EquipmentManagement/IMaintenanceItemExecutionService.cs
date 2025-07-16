using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 维护项目执行记录服务接口
    /// </summary>
    public interface IMaintenanceItemExecutionService : IService<MaintenanceItemExecution>
    {
        /// <summary>
        /// 根据维护执行ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId);
        
        /// <summary>
        /// 根据维护项目ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceItemId">维护项目ID</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceItemIdAsync(int maintenanceItemId);
        
        /// <summary>
        /// 根据合格状态获取维护项目执行记录
        /// </summary>
        /// <param name="isQualified">是否合格</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByQualifiedStatusAsync(bool isQualified);
        
        /// <summary>
        /// 根据执行人获取维护项目执行记录
        /// </summary>
        /// <param name="executorId">执行人ID</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByExecutorIdAsync(int executorId);
        
        /// <summary>
        /// 获取指定日期范围内的维护项目执行记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 获取特定维护工单的所有项目执行记录
        /// </summary>
        /// <param name="maintenanceOrderId">维护工单ID</param>
        /// <returns>维护项目执行记录列表</returns>
        Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceOrderIdAsync(int maintenanceOrderId);
        
        /// <summary>
        /// 批量添加维护项目执行记录
        /// </summary>
        /// <param name="executions">维护项目执行记录列表</param>
        /// <returns>添加的维护项目执行记录数量</returns>
        Task<int> AddRangeAsync(IEnumerable<MaintenanceItemExecution> executions);
    }
}