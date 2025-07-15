using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class BOMItemRepository : Repository<BOMItem>, IBOMItemRepository
    {
        public BOMItemRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取指定BOM的所有明细项
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetByBomIdAsync(int bomId)
        {
            return await _dbSet.Where(i => i.BomId == bomId)
                              .Include(i => i.Material)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取指定BOM的关键物料明细
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetKeyItemsByBomIdAsync(int bomId)
        {
            return await _dbSet.Where(i => i.BomId == bomId && i.IsKey)
                              .Include(i => i.Material)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取使用特定物料的所有BOM明细项
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetByMaterialIdAsync(int materialId)
        {
            return await _dbSet.Where(i => i.MaterialId == materialId)
                              .Include(i => i.BOM)
                              .ToListAsync();
        }
    }
}