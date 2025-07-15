using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IEquipmentRepository : IRepository<Equipment>
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
    }
}