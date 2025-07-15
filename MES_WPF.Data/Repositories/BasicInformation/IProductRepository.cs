using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IProductRepository : IRepository<Product>
    {
        /// <summary>
        /// 根据产品编码获取产品
        /// </summary>
        Task<Product> GetByCodeAsync(string productCode);
        
        /// <summary>
        /// 根据产品类型获取产品列表
        /// </summary>
        Task<IEnumerable<Product>> GetByProductTypeAsync(byte productType);
        
        /// <summary>
        /// 根据产品名称模糊查询
        /// </summary>
        Task<IEnumerable<Product>> SearchByNameAsync(string keyword);
    }
} 