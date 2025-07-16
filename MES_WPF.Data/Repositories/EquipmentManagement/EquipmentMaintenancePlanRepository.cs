using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class EquipmentMaintenancePlanRepository : Repository<EquipmentMaintenancePlan>, IEquipmentMaintenancePlanRepository
    {
        public EquipmentMaintenancePlanRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 获取特定设备的维护计划列表
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _dbSet.Where(p => p.EquipmentId == equipmentId).ToListAsync();
        }
        
        /// <summary>
        /// 获取需要执行的维护计划（下次执行日期小于等于指定日期）
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetPlansForExecutionAsync(DateTime date)
        {
            return await _dbSet
                .Where(p => p.NextExecuteDate <= date && p.Status == 1) // 1表示启用状态
                .OrderBy(p => p.NextExecuteDate)
                .ToListAsync();
        }
        
        /// <summary>
        /// 根据维护类型获取维护计划
        /// </summary>
        /// <param name="maintenanceType">维护类型</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetByMaintenanceTypeAsync(byte maintenanceType)
        {
            return await _dbSet.Where(p => p.MaintenanceType == maintenanceType).ToListAsync();
        }
        
        /// <summary>
        /// 根据计划编码获取维护计划
        /// </summary>
        /// <param name="planCode">计划编码</param>
        /// <returns>维护计划</returns>
        public async Task<EquipmentMaintenancePlan> GetByPlanCodeAsync(string planCode)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.PlanCode == planCode);
        }
        
        /// <summary>
        /// 更新计划的下一次执行日期
        /// </summary>
        /// <param name="id">维护计划ID</param>
        /// <param name="lastExecuteDate">上次执行日期</param>
        /// <returns>更新后的维护计划</returns>
        public async Task<EquipmentMaintenancePlan> UpdateExecuteDateAsync(int id, DateTime lastExecuteDate)
        {
            var plan = await _dbSet.FindAsync(id);
            if (plan == null)
                throw new ArgumentException($"找不到ID为{id}的维护计划", nameof(id));
            
            plan.LastExecuteDate = lastExecuteDate;
            
            // 根据周期类型和周期值计算下一次执行日期
            DateTime nextDate = lastExecuteDate;
            
            switch (plan.CycleType)
            {
                case 1: // 天
                    nextDate = lastExecuteDate.AddDays(plan.CycleValue);
                    break;
                case 2: // 周
                    nextDate = lastExecuteDate.AddDays(plan.CycleValue * 7);
                    break;
                case 3: // 月
                    nextDate = lastExecuteDate.AddMonths(plan.CycleValue);
                    break;
                case 4: // 季度
                    nextDate = lastExecuteDate.AddMonths(plan.CycleValue * 3);
                    break;
                case 5: // 年
                    nextDate = lastExecuteDate.AddYears(plan.CycleValue);
                    break;
            }
            
            plan.NextExecuteDate = nextDate;
            plan.UpdateTime = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            return plan;
        }
    }
} 