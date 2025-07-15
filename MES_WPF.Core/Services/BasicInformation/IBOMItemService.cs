using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// BOM明细服务接口
    /// </summary>
    public interface IBOMItemService : IService<BOMItem>
    {
        /// <summary>
        /// 获取指定BOM的所有明细项
        /// </summary>
        Task<IEnumerable<BOMItem>> GetByBomIdAsync(int bomId);
        
        /// <summary>
        /// 获取指定BOM的关键物料明细
        /// </summary>
        Task<IEnumerable<BOMItem>> GetKeyItemsByBomIdAsync(int bomId);
        
        /// <summary>
        /// 获取使用特定物料的所有BOM明细项
        /// </summary>
        Task<IEnumerable<BOMItem>> GetByMaterialIdAsync(int materialId);

        /// <summary>
        /// 批量添加BOM明细项
        /// </summary>
        Task<IEnumerable<BOMItem>> AddRangeAsync(IEnumerable<BOMItem> items);

        /// <summary>
        /// 删除指定BOM的所有明细项
        /// </summary>
        Task DeleteByBomIdAsync(int bomId);

        /// <summary>
        /// 更新BOM明细项的关键物料状态
        /// </summary>
        Task<BOMItem> UpdateKeyStatusAsync(int itemId, bool isKey);
    }
}