using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class MaintenanceExecutionRepository : Repository<MaintenanceExecution>, IMaintenanceExecutionRepository
    {
        private readonly MesDbContext _mesDbContext;
        
        public MaintenanceExecutionRepository(MesDbContext context) : base(context)
        {
            _mesDbContext = context;
        }
        
        /// <summary>
        /// 根据维护工单ID获取维护执行记录
        /// </summary>
        /// <param name="maintenanceOrderId">维护工单ID</param>
        /// <returns>维护执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceExecution>> GetByMaintenanceOrderIdAsync(int maintenanceOrderId)
        {
            return await _dbSet.Where(e => e.MaintenanceOrderId == maintenanceOrderId).ToListAsync();
        }
        
        /// <summary>
        /// 根据执行人ID获取维护执行记录
        /// </summary>
        /// <param name="executorId">执行人ID</param>
        /// <returns>维护执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceExecution>> GetByExecutorIdAsync(int executorId)
        {
            return await _dbSet.Where(e => e.ExecutorId == executorId).ToListAsync();
        }
        
        /// <summary>
        /// 根据执行结果获取维护执行记录
        /// </summary>
        /// <param name="executionResult">执行结果</param>
        /// <returns>维护执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceExecution>> GetByExecutionResultAsync(byte executionResult)
        {
            return await _dbSet.Where(e => e.ExecutionResult == executionResult).ToListAsync();
        }
        
        /// <summary>
        /// 获取指定日期范围内的维护执行记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(e => e.StartTime >= startDate && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取指定设备的维护执行记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceExecution>> GetByEquipmentIdAsync(int equipmentId)
        {
            var query = from execution in _dbSet
                        join order in _mesDbContext.Set<MaintenanceOrder>() on execution.MaintenanceOrderId equals order.Id
                        where order.EquipmentId == equipmentId
                        select execution;
                        
            return await query.ToListAsync();
        }
        
        /// <summary>
        /// 结束维护执行记录
        /// </summary>
        /// <param name="id">执行记录ID</param>
        /// <param name="executionResult">执行结果</param>
        /// <param name="resultDescription">结果描述</param>
        /// <returns>更新后的执行记录</returns>
        public async Task<MaintenanceExecution> CompleteExecutionAsync(int id, byte executionResult, string resultDescription)
        {
            var execution = await _dbSet.FindAsync(id);
            if (execution == null)
                throw new ArgumentException($"找不到ID为{id}的维护执行记录", nameof(id));
            
            execution.EndTime = DateTime.Now;
            execution.ExecutionResult = executionResult;
            execution.ResultDescription = resultDescription;
            
            // 计算工时（分钟）
            if (execution.StartTime != null && execution.EndTime.HasValue)
            {
                execution.LaborTime = (decimal)(execution.EndTime.Value - execution.StartTime).TotalMinutes;
            }
            
            execution.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return execution;
        }
    }
} 