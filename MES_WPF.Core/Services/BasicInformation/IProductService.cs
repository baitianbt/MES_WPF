using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 产品服务接口
    /// </summary>
    public interface IProductService : IService<Product>
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

        /// <summary>
        /// 判断产品编码是否已存在
        /// </summary>
        Task<bool> IsProductCodeExistsAsync(string productCode);
        
        /// <summary>
        /// 启用/禁用产品
        /// </summary>
        Task<Product> ToggleProductStatusAsync(int productId, bool isActive);
    }
}