using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 备件使用记录服务实现
    /// </summary>
    public class SpareUsageService : Service<SpareUsage>, ISpareUsageService
    {
        private readonly ISpareUsageRepository _spareUsageRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">备件使用记录仓储</param>
        public SpareUsageService(ISpareUsageRepository repository) : base(repository)
        {
            _spareUsageRepository = repository;
        }
        
        /// <summary>
        /// 根据维护执行ID获取备件使用记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId)
        {
            return await _spareUsageRepository.GetByMaintenanceExecutionIdAsync(maintenanceExecutionId);
        }
        
        /// <summary>
        /// 根据备件ID获取备件使用记录
        /// </summary>
        /// <param name="spareId">备件ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetBySpareIdAsync(int spareId)
        {
            return await _spareUsageRepository.GetBySpareIdAsync(spareId);
        }
        
        /// <summary>
        /// 根据使用类型获取备件使用记录
        /// </summary>
        /// <param name="usageType">使用类型</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByUsageTypeAsync(byte usageType)
        {
            return await _spareUsageRepository.GetByUsageTypeAsync(usageType);
        }
        
        /// <summary>
        /// 根据操作人获取备件使用记录
        /// </summary>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByOperatorIdAsync(int operatorId)
        {
            return await _spareUsageRepository.GetByOperatorIdAsync(operatorId);
        }
        
        /// <summary>
        /// 获取指定日期范围内的备件使用记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _spareUsageRepository.GetByDateRangeAsync(startDate, endDate);
        }
        
        /// <summary>
        /// 获取特定设备的备件使用记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>备件使用记录列表</returns>
        public async Task<IEnumerable<SpareUsage>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _spareUsageRepository.GetByEquipmentIdAsync(equipmentId);
        }
        
        /// <summary>
        /// 添加备件使用记录并更新备件库存
        /// </summary>
        /// <param name="spareUsage">备件使用记录</param>
        /// <returns>添加的备件使用记录</returns>
        public async Task<SpareUsage> AddUsageAndUpdateStockAsync(SpareUsage spareUsage)
        {
            return await _spareUsageRepository.AddUsageAndUpdateStockAsync(spareUsage);
        }
    }
}