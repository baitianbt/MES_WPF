using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IBOMItemRepository : IRepository<BOMItem>
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
    }
}