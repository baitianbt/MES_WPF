using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public interface ISpareUsageRepository : IRepository<SpareUsage>
    {
        /// <summary>
        /// 根据维护执行ID获取备件使用记录
        /// </summary>
        /// <param name="maintenanceExecutionId">维护执行ID</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetByMaintenanceExecutionIdAsync(int maintenanceExecutionId);
        
        /// <summary>
        /// 根据备件ID获取备件使用记录
        /// </summary>
        /// <param name="spareId">备件ID</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetBySpareIdAsync(int spareId);
        
        /// <summary>
        /// 根据使用类型获取备件使用记录
        /// </summary>
        /// <param name="usageType">使用类型</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetByUsageTypeAsync(byte usageType);
        
        /// <summary>
        /// 根据操作人获取备件使用记录
        /// </summary>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetByOperatorIdAsync(int operatorId);
        
        /// <summary>
        /// 获取指定日期范围内的备件使用记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 获取特定设备的备件使用记录
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>备件使用记录列表</returns>
        Task<IEnumerable<SpareUsage>> GetByEquipmentIdAsync(int equipmentId);
        
        /// <summary>
        /// 添加备件使用记录并更新备件库存
        /// </summary>
        /// <param name="spareUsage">备件使用记录</param>
        /// <returns>添加的备件使用记录</returns>
        Task<SpareUsage> AddUsageAndUpdateStockAsync(SpareUsage spareUsage);
    }
}