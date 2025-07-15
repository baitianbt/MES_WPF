using MES_WPF.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 操作日志服务接口
    /// </summary>
    public interface IOperationLogService : IService<OperationLog>
    {
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <param name="operationType">操作类型</param>
        /// <param name="operationDesc">操作描述</param>
        /// <param name="requestMethod">请求方法</param>
        /// <param name="requestUrl">请求URL</param>
        /// <param name="requestParams">请求参数</param>
        /// <param name="responseResult">响应结果</param>
        /// <param name="operationUser">操作用户ID</param>
        /// <param name="operationIp">操作IP</param>
        /// <param name="executionTime">执行时长(毫秒)</param>
        /// <param name="status">状态(1:成功,0:失败)</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>操作日志</returns>
        Task<OperationLog> AddLogAsync(
            string moduleType,
            string operationType,
            string operationDesc,
            string requestMethod,
            string requestUrl,
            string requestParams,
            string responseResult,
            int operationUser,
            string operationIp,
            int? executionTime,
            byte status,
            string errorMsg = null);
        
        /// <summary>
        /// 获取用户操作日志
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>操作日志列表</returns>
        Task<IEnumerable<OperationLog>> GetUserLogsAsync(int userId);
        
        /// <summary>
        /// 获取指定时间范围内的操作日志
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>操作日志列表</returns>
        Task<IEnumerable<OperationLog>> GetLogsByTimeRangeAsync(DateTime startTime, DateTime endTime);
        
        /// <summary>
        /// 获取指定模块的操作日志
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>操作日志列表</returns>
        Task<IEnumerable<OperationLog>> GetLogsByModuleTypeAsync(string moduleType);
        
        /// <summary>
        /// 获取指定操作类型的操作日志
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <returns>操作日志列表</returns>
        Task<IEnumerable<OperationLog>> GetLogsByOperationTypeAsync(string operationType);
        
        /// <summary>
        /// 清理指定日期之前的操作日志
        /// </summary>
        /// <param name="beforeDate">日期</param>
        /// <returns>清理的日志数量</returns>
        Task<int> ClearLogsBeforeDateAsync(DateTime beforeDate);
    }
} 