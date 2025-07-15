using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class OperationLogRepository : Repository<OperationLog>, IOperationLogRepository
    {
        public OperationLogRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据条件筛选操作日志
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="operationUser">操作用户ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>包含操作日志列表和总数的元组</returns>
        public async Task<(IEnumerable<OperationLog> logs, int totalCount)> SearchAsync(
            string moduleType = null,
            string operationType = null,
            int? operationUser = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            byte? status = null,
            int pageIndex = 1,
            int pageSize = 20)
        {
            var query = _dbSet.AsQueryable();
            
            // 应用筛选条件
            if (!string.IsNullOrWhiteSpace(moduleType))
            {
                query = query.Where(ol => ol.ModuleType == moduleType);
            }
            
            if (!string.IsNullOrWhiteSpace(operationType))
            {
                query = query.Where(ol => ol.OperationType == operationType);
            }
            
            if (operationUser.HasValue)
            {
                query = query.Where(ol => ol.OperationUser == operationUser.Value);
            }
            
            if (startTime.HasValue)
            {
                query = query.Where(ol => ol.OperationTime >= startTime.Value);
            }
            
            if (endTime.HasValue)
            {
                query = query.Where(ol => ol.OperationTime <= endTime.Value);
            }
            
            if (status.HasValue)
            {
                query = query.Where(ol => ol.Status == status.Value);
            }
            
            // 获取总数
            int totalCount = await query.CountAsync();
            
            // 分页并返回结果
            var logs = await query
                .OrderByDescending(ol => ol.OperationTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return (logs, totalCount);
        }
        
        /// <summary>
        /// 清理指定日期之前的操作日志
        /// </summary>
        /// <param name="beforeDate">日期</param>
        /// <returns>清理的记录数</returns>
        public async Task<int> CleanupLogsAsync(DateTime beforeDate)
        {
            // 获取指定日期之前的日志
            var logs = await _dbSet
                .Where(ol => ol.OperationTime < beforeDate)
                .ToListAsync();
            
            // 删除日志
            _context.RemoveRange(logs);
            await _context.SaveChangesAsync();
            
            return logs.Count;
        }
    }
} 