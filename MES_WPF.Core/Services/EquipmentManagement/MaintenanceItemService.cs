using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 维护项目服务实现
    /// </summary>
    public class MaintenanceItemService : Service<MaintenanceItem>, IMaintenanceItemService
    {
        private readonly IMaintenanceItemRepository _maintenanceItemRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">维护项目仓储</param>
        public MaintenanceItemService(IMaintenanceItemRepository repository) : base(repository)
        {
            _maintenanceItemRepository = repository;
        }
        
        /// <summary>
        /// 获取特定维护计划的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetByMaintenancePlanIdAsync(int maintenancePlanId)
        {
            return await _maintenanceItemRepository.GetByMaintenancePlanIdAsync(maintenancePlanId);
        }
        
        /// <summary>
        /// 根据项目编码获取维护项目
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>维护项目</returns>
        public async Task<MaintenanceItem> GetByItemCodeAsync(string itemCode)
        {
            return await _maintenanceItemRepository.GetByItemCodeAsync(itemCode);
        }
        
        /// <summary>
        /// 根据项目类型获取维护项目
        /// </summary>
        /// <param name="itemType">项目类型</param>
        /// <returns>维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetByItemTypeAsync(byte itemType)
        {
            return await _maintenanceItemRepository.GetByItemTypeAsync(itemType);
        }
        
        /// <summary>
        /// 获取特定维护计划按序号排序的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>排序后的维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetSortedItemsByPlanIdAsync(int maintenancePlanId)
        {
            return await _maintenanceItemRepository.GetSortedItemsByPlanIdAsync(maintenancePlanId);
        }
    }
}