using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class BOMRepository : Repository<BOM>, IBOMRepository
    {
        public BOMRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据BOM编码获取BOM
        /// </summary>
        public async Task<BOM> GetByCodeAsync(string bomCode)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.BomCode == bomCode);
        }

        /// <summary>
        /// 获取指定产品的BOM列表
        /// </summary>
        public async Task<IEnumerable<BOM>> GetByProductIdAsync(int productId)
        {
            return await _dbSet.Where(b => b.ProductId == productId).ToListAsync();
        }

        /// <summary>
        /// 获取指定产品的默认BOM
        /// </summary>
        public async Task<BOM> GetDefaultByProductIdAsync(int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.ProductId == productId && b.IsDefault);
        }

        /// <summary>
        /// 获取指定状态的BOM列表
        /// </summary>
        public async Task<IEnumerable<BOM>> GetByStatusAsync(byte status)
        {
            return await _dbSet.Where(b => b.Status == status).ToListAsync();
        }
    }
} 