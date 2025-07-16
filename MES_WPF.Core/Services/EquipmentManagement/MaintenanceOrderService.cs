using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 维护工单服务实现
    /// </summary>
    public class MaintenanceOrderService : Service<MaintenanceOrder>, IMaintenanceOrderService
    {
        private readonly IMaintenanceOrderRepository _maintenanceOrderRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">维护工单仓储</param>
        public MaintenanceOrderService(IMaintenanceOrderRepository repository) : base(repository)
        {
            _maintenanceOrderRepository = repository;
        }
        
        /// <summary>
        /// 根据工单编码获取维护工单
        /// </summary>
        /// <param name="orderCode">工单编码</param>
        /// <returns>维护工单</returns>
        public async Task<MaintenanceOrder> GetByOrderCodeAsync(string orderCode)
        {
            return await _maintenanceOrderRepository.GetByOrderCodeAsync(orderCode);
        }
        
        /// <summary>
        /// 根据设备ID获取维护工单
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _maintenanceOrderRepository.GetByEquipmentIdAsync(equipmentId);
        }
        
        /// <summary>
        /// 根据维护计划ID获取维护工单
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByMaintenancePlanIdAsync(int maintenancePlanId)
        {
            return await _maintenanceOrderRepository.GetByMaintenancePlanIdAsync(maintenancePlanId);
        }
        
        /// <summary>
        /// 根据工单类型获取维护工单
        /// </summary>
        /// <param name="orderType">工单类型</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByOrderTypeAsync(byte orderType)
        {
            return await _maintenanceOrderRepository.GetByOrderTypeAsync(orderType);
        }
        
        /// <summary>
        /// 根据工单状态获取维护工单
        /// </summary>
        /// <param name="status">工单状态</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByStatusAsync(byte status)
        {
            return await _maintenanceOrderRepository.GetByStatusAsync(status);
        }
        
        /// <summary>
        /// 根据报修人获取维护工单
        /// </summary>
        /// <param name="reportBy">报修人ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByReportByAsync(int reportBy)
        {
            return await _maintenanceOrderRepository.GetByReportByAsync(reportBy);
        }
        
        /// <summary>
        /// 根据分配人获取维护工单
        /// </summary>
        /// <param name="assignedTo">分配人ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByAssignedToAsync(int assignedTo)
        {
            return await _maintenanceOrderRepository.GetByAssignedToAsync(assignedTo);
        }
        
        /// <summary>
        /// 获取指定日期范围内的维护工单
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _maintenanceOrderRepository.GetByDateRangeAsync(startDate, endDate);
        }
        
        /// <summary>
        /// 更新工单状态
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的工单</returns>
        public async Task<MaintenanceOrder> UpdateStatusAsync(int id, byte status)
        {
            return await _maintenanceOrderRepository.UpdateStatusAsync(id, status);
        }
        
        /// <summary>
        /// 分配工单给维护人员
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="assignedTo">维护人员ID</param>
        /// <returns>更新后的工单</returns>
        public async Task<MaintenanceOrder> AssignOrderAsync(int id, int assignedTo)
        {
            return await _maintenanceOrderRepository.AssignOrderAsync(id, assignedTo);
        }
    }
}