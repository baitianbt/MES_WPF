using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据产品编码获取产品
        /// </summary>
        public async Task<Product> GetByCodeAsync(string productCode)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }

        /// <summary>
        /// 根据产品类型获取产品列表
        /// </summary>
        public async Task<IEnumerable<Product>> GetByProductTypeAsync(byte productType)
        {
            return await _dbSet.Where(p => p.ProductType == productType).ToListAsync();
        }

        /// <summary>
        /// 根据产品名称模糊查询
        /// </summary>
        public async Task<IEnumerable<Product>> SearchByNameAsync(string keyword)
        {
            return await _dbSet.Where(p => p.ProductName.Contains(keyword)).ToListAsync();
        }
    }
}