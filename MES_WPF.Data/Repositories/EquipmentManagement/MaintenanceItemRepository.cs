using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class MaintenanceItemRepository : Repository<MaintenanceItem>, IMaintenanceItemRepository
    {
        public MaintenanceItemRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 获取特定维护计划的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetByMaintenancePlanIdAsync(int maintenancePlanId)
        {
            return await _dbSet.Where(item => item.MaintenancePlanId == maintenancePlanId).ToListAsync();
        }
        
        /// <summary>
        /// 根据项目编码获取维护项目
        /// </summary>
        /// <param name="itemCode">项目编码</param>
        /// <returns>维护项目</returns>
        public async Task<MaintenanceItem> GetByItemCodeAsync(string itemCode)
        {
            return await _dbSet.FirstOrDefaultAsync(item => item.ItemCode == itemCode);
        }
        
        /// <summary>
        /// 根据项目类型获取维护项目
        /// </summary>
        /// <param name="itemType">项目类型</param>
        /// <returns>维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetByItemTypeAsync(byte itemType)
        {
            return await _dbSet.Where(item => item.ItemType == itemType).ToListAsync();
        }
        
        /// <summary>
        /// 获取特定维护计划按序号排序的项目列表
        /// </summary>
        /// <param name="maintenancePlanId">维护计划ID</param>
        /// <returns>排序后的维护项目列表</returns>
        public async Task<IEnumerable<MaintenanceItem>> GetSortedItemsByPlanIdAsync(int maintenancePlanId)
        {
            return await _dbSet
                .Where(item => item.MaintenancePlanId == maintenancePlanId)
                .OrderBy(item => item.SequenceNo)
                .ToListAsync();
        }
    }
} 