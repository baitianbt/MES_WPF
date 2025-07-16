using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public interface IMaintenanceOrderRepository : IRepository<MaintenanceOrder>
    {
        /// <summary>
        /// 根据工单编码获取维护工单
        /// </summary>
        /// <param name="orderCode">工单编码</param>
        /// <returns>维护工单</returns>
        Task<MaintenanceOrder> GetByOrderCodeAsync(string orderCode);
        
        /// <summary>
        /// 根据设备ID获取维护工单
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByEquipmentIdAsync(int equipmentId);
        
        /// <summary>
        /// 根据维护计划ID获取维护工单
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByMaintenancePlanIdAsync(int maintenancePlanId);
        
        /// <summary>
        /// 根据工单类型获取维护工单
        /// </summary>
        /// <param name="orderType">工单类型</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByOrderTypeAsync(byte orderType);
        
        /// <summary>
        /// 根据工单状态获取维护工单
        /// </summary>
        /// <param name="status">工单状态</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByStatusAsync(byte status);
        
        /// <summary>
        /// 根据报修人获取维护工单
        /// </summary>
        /// <param name="reportBy">报修人ID</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByReportByAsync(int reportBy);
        
        /// <summary>
        /// 根据分配人获取维护工单
        /// </summary>
        /// <param name="assignedTo">分配人ID</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByAssignedToAsync(int assignedTo);
        
        /// <summary>
        /// 获取指定日期范围内的维护工单
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护工单列表</returns>
        Task<IEnumerable<MaintenanceOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 更新工单状态
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的工单</returns>
        Task<MaintenanceOrder> UpdateStatusAsync(int id, byte status);
        
        /// <summary>
        /// 分配工单给维护人员
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="assignedTo">维护人员ID</param>
        /// <returns>更新后的工单</returns>
        Task<MaintenanceOrder> AssignOrderAsync(int id, int assignedTo);
    }
}