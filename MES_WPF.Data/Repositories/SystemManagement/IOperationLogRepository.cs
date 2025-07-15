using MES_WPF.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IOperationLogRepository : IRepository<OperationLog>
    {
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
        Task<(IEnumerable<OperationLog> logs, int totalCount)> SearchAsync(
            string moduleType = null,
            string operationType = null,
            int? operationUser = null,
            DateTime? startTime = null,
            DateTime? endTime = null,
            byte? status = null,
            int pageIndex = 1,
            int pageSize = 20);
            
        /// <summary>
        /// 清理指定日期之前的操作日志
        /// </summary>
        /// <param name="beforeDate">日期</param>
        /// <returns>清理的记录数</returns>
        Task<int> CleanupLogsAsync(DateTime beforeDate);
    }
} 