using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class SpareUsageRepository : Repository<SpareUsage>, ISpareUsageRepository
    {
        private readonly MesDbContext _mesDbContext;
        
        public SpareUsageRepository(MesDbContext context) : base(context)
        {
            _mesDbContext = context;
        }
        
        /// <summary>
        /// 根据维护执行ID获取备件使用记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId)
        {
            return await _dbSet.Where(u => u.MaintenanceExecutionId == maintenanceExecutionId).ToListAsync();
        }
        
        /// <summary>
        /// 根据备件ID获取备件使用记录
        /// </summary>
        /// <param name="spareId">备件ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetBySpareIdAsync(int spareId)
        {
            return await _dbSet.Where(u => u.SpareId == spareId).ToListAsync();
        }
        
        /// <summary>
        /// 根据使用类型获取备件使用记录
        /// </summary>
        /// <param name="usageType">使用类型</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByUsageTypeAsync(byte usageType)
        {
            return await _dbSet.Where(u => u.UsageType == usageType).ToListAsync();
        }
        
        /// <summary>
        /// 根据操作人获取备件使用记录
        /// </summary>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByOperatorIdAsync(int operatorId)
        {
            return await _dbSet.Where(u => u.OperatorId == operatorId).ToListAsync();
        }
        
        /// <summary>
        /// 获取指定日期范围内的备件使用记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(u => u.UsageTime >= startDate && u.UsageTime <= endDate)
                .OrderBy(u => u.UsageTime)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取特定设备的备件使用记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByEquipmentIdAsync(int equipmentId)
        {
            var query = from usage in _dbSet
                        join execution in _mesDbContext.Set<MaintenanceExecution>() 
                            on usage.MaintenanceExecutionId equals execution.Id
                        join order in _mesDbContext.Set<MaintenanceOrder>() 
                            on execution.MaintenanceOrderId equals order.Id
                        where order.EquipmentId == equipmentId
                        select usage;
                        
            return await query.ToListAsync();
        }
        
        /// <summary>
        /// 添加备件使用记录并更新备件库存
        /// </summary>
        /// <param name="spareUsage">备件使用记录</param>
        /// <returns>添加的备件使用记录</returns>
        public async Task<SpareUsage> AddUsageAndUpdateStockAsync(SpareUsage spareUsage)
        {
            // 开始事务
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 添加使用记录
                    await _dbSet.AddAsync(spareUsage);
                    await _context.SaveChangesAsync();
                    
                    // 更新备件库存（根据使用类型确定增减）
                    var spare = await _mesDbContext.Set<Spare>().FindAsync(spareUsage.SpareId);
                    if (spare == null)
                        throw new ArgumentException($"找不到ID为{spareUsage.SpareId}的备件", nameof(spareUsage.SpareId));
                    
                    // 根据使用类型更新库存
                    decimal quantityChange = 0;
                    switch (spareUsage.UsageType)
                    {
                        case 1: // 更换
                        case 3: // 消耗
                            quantityChange = -spareUsage.Quantity; // 减少库存
                            break;
                        case 2: // 添加
                            quantityChange = spareUsage.Quantity; // 增加库存
                            break;
                    }
                    
                    // 检查库存是否足够
                    if (quantityChange < 0 && spare.StockQuantity < Math.Abs(quantityChange))
                        throw new InvalidOperationException($"备件{spare.SpareName}库存不足，当前库存: {spare.StockQuantity}，需要: {Math.Abs(quantityChange)}");
                    
                    spare.StockQuantity += quantityChange;
                    spare.UpdateTime = DateTime.Now;
                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    return spareUsage;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}