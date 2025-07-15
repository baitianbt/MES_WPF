using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 产品服务实现
    /// </summary>
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProductService(IProductRepository productRepository) : base(productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// 根据产品编码获取产品
        /// </summary>
        public async Task<Product> GetByCodeAsync(string productCode)
        {
            return await _productRepository.GetByCodeAsync(productCode);
        }

        /// <summary>
        /// 根据产品类型获取产品列表
        /// </summary>
        public async Task<IEnumerable<Product>> GetByProductTypeAsync(byte productType)
        {
            return await _productRepository.GetByProductTypeAsync(productType);
        }

        /// <summary>
        /// 根据产品名称模糊查询
        /// </summary>
        public async Task<IEnumerable<Product>> SearchByNameAsync(string keyword)
        {
            return await _productRepository.SearchByNameAsync(keyword);
        }

        /// <summary>
        /// 判断产品编码是否已存在
        /// </summary>
        public async Task<bool> IsProductCodeExistsAsync(string productCode)
        {
            var product = await _productRepository.GetByCodeAsync(productCode);
            return product != null;
        }

        /// <summary>
        /// 启用/禁用产品
        /// </summary>
        public async Task<Product> ToggleProductStatusAsync(int productId, bool isActive)
        {
            var product = await GetByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException($"产品ID {productId} 不存在");
            }

            product.IsActive = isActive;
            product.UpdateTime = DateTime.Now;
            
            return await UpdateAsync(product);
        }
    }
}