using MES_WPF.Model.EquipmentManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public interface IMaintenanceItemRepository : IRepository<MaintenanceItem>
    {
        /// <summary>
        /// 获取特定维护计划的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护项目列表</returns>
        Task<IEnumerable<MaintenanceItem>> GetByMaintenancePlanIdAsync(int maintenancePlanId);
        
        /// <summary>
        /// 根据项目编码获取维护项目
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>维护项目</returns>
        Task<MaintenanceItem> GetByItemCodeAsync(string itemCode);
        
        /// <summary>
        /// 根据项目类型获取维护项目
        /// </summary>
        /// <param name="itemType">项目类型</param>
        /// <returns>维护项目列表</returns>
        Task<IEnumerable<MaintenanceItem>> GetByItemTypeAsync(byte itemType);
        
        /// <summary>
        /// 获取特定维护计划按序号排序的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>排序后的维护项目列表</returns>
        Task<IEnumerable<MaintenanceItem>> GetSortedItemsByPlanIdAsync(int maintenancePlanId);
    }
}