using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class MaintenanceOrderRepository : Repository<MaintenanceOrder>, IMaintenanceOrderRepository
    {
        public MaintenanceOrderRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 根据工单编码获取维护工单
        /// </summary>
        /// <param name="orderCode">工单编码</param>
        /// <returns>维护工单</returns>
        public async Task<MaintenanceOrder> GetByOrderCodeAsync(string orderCode)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }
        
        /// <summary>
        /// 根据设备ID获取维护工单
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _dbSet.Where(o => o.EquipmentId == equipmentId).ToListAsync();
        }
        
        /// <summary>
        /// 根据维护计划ID获取维护工单
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByMaintenancePlanIdAsync(int maintenancePlanId)
        {
            return await _dbSet.Where(o => o.MaintenancePlanId == maintenancePlanId).ToListAsync();
        }
        
        /// <summary>
        /// 根据工单类型获取维护工单
        /// </summary>
        /// <param name="orderType">工单类型</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByOrderTypeAsync(byte orderType)
        {
            return await _dbSet.Where(o => o.OrderType == orderType).ToListAsync();
        }
        
        /// <summary>
        /// 根据工单状态获取维护工单
        /// </summary>
        /// <param name="status">工单状态</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByStatusAsync(byte status)
        {
            return await _dbSet.Where(o => o.Status == status).ToListAsync();
        }
        
        /// <summary>
        /// 根据报修人获取维护工单
        /// </summary>
        /// <param name="reportBy">报修人ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByReportByAsync(int reportBy)
        {
            return await _dbSet.Where(o => o.ReportBy == reportBy).ToListAsync();
        }
        
        /// <summary>
        /// 根据分配人获取维护工单
        /// </summary>
        /// <param name="assignedTo">分配人ID</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByAssignedToAsync(int assignedTo)
        {
            return await _dbSet.Where(o => o.AssignedTo == assignedTo).ToListAsync();
        }
        
        /// <summary>
        /// 获取指定日期范围内的维护工单
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护工单列表</returns>
        public async Task<IEnumerable<MaintenanceOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(o => o.PlanStartTime >= startDate && o.PlanEndTime <= endDate)
                .OrderBy(o => o.PlanStartTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 更新工单状态
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="status">新状态</param>
        /// <returns>更新后的工单</returns>
        public async Task<MaintenanceOrder> UpdateStatusAsync(int id, byte status)
        {
            var order = await _dbSet.FindAsync(id);
            if (order == null)
                throw new ArgumentException($"找不到ID为{id}的维护工单", nameof(id));
            
            order.Status = status;
            
            // 如果状态为处理中，设置实际开始时间
            if (status == 3 && order.ActualStartTime == null)
            {
                order.ActualStartTime = DateTime.Now;
            }
            // 如果状态为已完成，设置实际结束时间
            else if (status == 4 && order.ActualEndTime == null)
            {
                order.ActualEndTime = DateTime.Now;
            }
            
            order.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return order;
        }
        
        /// <summary>
        /// 分配工单给维护人员
        /// </summary>
        /// <param name="id">工单ID</param>
        /// <param name="assignedTo">维护人员ID</param>
        /// <returns>更新后的工单</returns>
        public async Task<MaintenanceOrder> AssignOrderAsync(int id, int assignedTo)
        {
            var order = await _dbSet.FindAsync(id);
            if (order == null)
                throw new ArgumentException($"找不到ID为{id}的维护工单", nameof(id));
            
            order.AssignedTo = assignedTo;
            // 如果当前状态为待处理，则更新为已分配
            if (order.Status == 1)
            {
                order.Status = 2; // 已分配
            }
            
            order.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return order;
        }
    }
}