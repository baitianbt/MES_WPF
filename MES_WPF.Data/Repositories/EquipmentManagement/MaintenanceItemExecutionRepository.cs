using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class MaintenanceItemExecutionRepository : Repository<MaintenanceItemExecution>, IMaintenanceItemExecutionRepository
    {
        private readonly MesDbContext _mesDbContext;
        
        public MaintenanceItemExecutionRepository(MesDbContext context) : base(context)
        {
            _mesDbContext = context;
        }
        
        /// <summary>
        /// 根据维护执行ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId)
        {
            return await _dbSet.Where(e => e.MaintenanceExecutionId == maintenanceExecutionId).ToListAsync();
        }
        
        /// <summary>
        /// 根据维护项目ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceItemId">维护项目ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceItemIdAsync(int maintenanceItemId)
        {
            return await _dbSet.Where(e => e.MaintenanceItemId == maintenanceItemId).ToListAsync();
        }
        
        /// <summary>
        /// 根据合格状态获取维护项目执行记录
        /// </summary>
        /// <param name="isQualified">是否合格</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByQualifiedStatusAsync(bool isQualified)
        {
            return await _dbSet.Where(e => e.IsQualified == isQualified).ToListAsync();
        }
        
        /// <summary>
        /// 根据执行人获取维护项目执行记录
        /// </summary>
        /// <param name="executorId">执行人ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByExecutorIdAsync(int executorId)
        {
            return await _dbSet.Where(e => e.ExecutorId == executorId).ToListAsync();
        }
        
        /// <summary>
        /// 获取指定日期范围内的维护项目执行记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(e => e.ExecutionTime >= startDate && e.ExecutionTime <= endDate)
                .OrderBy(e => e.ExecutionTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取特定维护工单的所有项目执行记录
        /// </summary>
        /// <param name="maintenanceOrderId">维护工单ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceOrderIdAsync(int maintenanceOrderId)
        {
            var query = from itemExecution in _dbSet
                        join execution in _mesDbContext.Set<MaintenanceExecution>() 
                            on itemExecution.MaintenanceExecutionId equals execution.Id
                        where execution.MaintenanceOrderId == maintenanceOrderId
                        select itemExecution;
                        
            return await query.ToListAsync();
        }
        
        /// <summary>
        /// 批量添加维护项目执行记录
        /// </summary>
        /// <param name="executions">维护项目执行记录列表</param>
        /// <returns>添加的维护项目执行记录数量</returns>
        public async Task<int> AddRangeAsync(IEnumerable<MaintenanceItemExecution> executions)
        {
            await _dbSet.AddRangeAsync(executions);
            return await _context.SaveChangesAsync();
        }
    }
}