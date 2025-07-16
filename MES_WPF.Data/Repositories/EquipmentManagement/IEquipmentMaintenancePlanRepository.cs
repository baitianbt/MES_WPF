using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public interface IEquipmentMaintenancePlanRepository : IRepository<EquipmentMaintenancePlan>
    {
        /// <summary>
        /// 获取特定设备的维护计划列表
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护计划列表</returns>
        Task<IEnumerable<EquipmentMaintenancePlan>> GetByEquipmentIdAsync(int equipmentId);
        
        /// <summary>
        /// 获取需要执行的维护计划（下次执行日期小于等于指定日期）
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>维护计划列表</returns>
        Task<IEnumerable<EquipmentMaintenancePlan>> GetPlansForExecutionAsync(DateTime date);
        
        /// <summary>
        /// 根据维护类型获取维护计划
        /// </summary>
        /// <param name="maintenanceType">维护类型</param>
        /// <returns>维护计划列表</returns>
        Task<IEnumerable<EquipmentMaintenancePlan>> GetByMaintenanceTypeAsync(byte maintenanceType);
        
        /// <summary>
        /// 根据计划编码获取维护计划
        /// </summary>
        /// <param name="planCode">计划编码</param>
        /// <returns>维护计划</returns>
        Task<EquipmentMaintenancePlan> GetByPlanCodeAsync(string planCode);
        
        /// <summary>
        /// 更新计划的下一次执行日期
        /// </summary>
        /// <param name="id">维护计划ID</param>
        /// <param name="lastExecuteDate">上次执行日期</param>
        /// <returns>更新后的维护计划</returns>
        Task<EquipmentMaintenancePlan> UpdateExecuteDateAsync(int id, DateTime lastExecuteDate);
    }
}