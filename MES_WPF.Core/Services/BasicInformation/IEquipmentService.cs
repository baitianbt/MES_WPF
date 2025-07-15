using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 设备服务接口
    /// </summary>
    public interface IEquipmentService : IService<Equipment>
    {
        /// <summary>
        /// 根据资源ID获取设备
        /// </summary>
        Task<Equipment> GetByResourceIdAsync(int resourceId);
        
        /// <summary>
        /// 根据序列号获取设备
        /// </summary>
        Task<Equipment> GetBySerialNumberAsync(string serialNumber);
        
        /// <summary>
        /// 获取需要维护的设备（下次维护日期小于等于当前日期）
        /// </summary>
        Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync();
        
        /// <summary>
        /// 获取特定日期范围内需要维护的设备
        /// </summary>
        Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 记录设备维护
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>更新后的设备</returns>
        Task<Equipment> RecordMaintenanceAsync(int equipmentId);

        /// <summary>
        /// 检查序列号是否存在
        /// </summary>
        Task<bool> IsSerialNumberExistsAsync(string serialNumber);

        /// <summary>
        /// 更新设备保养周期
        /// </summary>
        Task<Equipment> UpdateMaintenanceCycleAsync(int equipmentId, int maintenanceCycle);
    }
} 