using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 维护项目执行记录服务实现
    /// </summary>
    public class MaintenanceItemExecutionService : Service<MaintenanceItemExecution>, IMaintenanceItemExecutionService
    {
        private readonly IMaintenanceItemExecutionRepository _maintenanceItemExecutionRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">维护项目执行记录仓储</param>
        public MaintenanceItemExecutionService(IMaintenanceItemExecutionRepository repository) : base(repository)
        {
            _maintenanceItemExecutionRepository = repository;
        }
        
        /// <summary>
        /// 根据维护执行ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId)
        {
            return await _maintenanceItemExecutionRepository.GetByMaintenanceExecutionIdAsync(maintenanceExecutionId);
        }
        
        /// <summary>
        /// 根据维护项目ID获取维护项目执行记录
        /// </summary>
        /// <param name="maintenanceItemId">维护项目ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceItemIdAsync(int maintenanceItemId)
        {
            return await _maintenanceItemExecutionRepository.GetByMaintenanceItemIdAsync(maintenanceItemId);
        }
        
        /// <summary>
        /// 根据合格状态获取维护项目执行记录
        /// </summary>
        /// <param name="isQualified">是否合格</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByQualifiedStatusAsync(bool isQualified)
        {
            return await _maintenanceItemExecutionRepository.GetByQualifiedStatusAsync(isQualified);
        }
        
        /// <summary>
        /// 根据执行人获取维护项目执行记录
        /// </summary>
        /// <param name="executorId">执行人ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByExecutorIdAsync(int executorId)
        {
            return await _maintenanceItemExecutionRepository.GetByExecutorIdAsync(executorId);
        }
        
        /// <summary>
        /// 获取指定日期范围内的维护项目执行记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _maintenanceItemExecutionRepository.GetByDateRangeAsync(startDate, endDate);
        }
        
        /// <summary>
        /// 获取特定维护工单的所有项目执行记录
        /// </summary>
        /// <param name="maintenanceOrderId">维护工单ID</param>
        /// <returns>维护项目执行记录列表</returns>
        public async Task<IEnumerable<MaintenanceItemExecution>> GetByMaintenanceOrderIdAsync(int maintenanceOrderId)
        {
            return await _maintenanceItemExecutionRepository.GetByMaintenanceOrderIdAsync(maintenanceOrderId);
        }
        
        /// <summary>
        /// 批量添加维护项目执行记录
        /// </summary>
        /// <param name="executions">维护项目执行记录列表</param>
        /// <returns>添加的维护项目执行记录数量</returns>
        public async Task<int> AddRangeAsync(IEnumerable<MaintenanceItemExecution> executions)
        {
            return await _maintenanceItemExecutionRepository.AddRangeAsync(executions);
        }
    }
}