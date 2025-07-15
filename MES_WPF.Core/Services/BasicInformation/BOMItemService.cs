using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// BOM明细服务实现
    /// </summary>
    public class BOMItemService : Service<BOMItem>, IBOMItemService
    {
        private readonly IBOMItemRepository _bomItemRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BOMItemService(IBOMItemRepository bomItemRepository) : base(bomItemRepository)
        {
            _bomItemRepository = bomItemRepository;
        }

        /// <summary>
        /// 获取指定BOM的所有明细项
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetByBomIdAsync(int bomId)
        {
            return await _bomItemRepository.GetByBomIdAsync(bomId);
        }

        /// <summary>
        /// 获取指定BOM的关键物料明细
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetKeyItemsByBomIdAsync(int bomId)
        {
            return await _bomItemRepository.GetKeyItemsByBomIdAsync(bomId);
        }

        /// <summary>
        /// 获取使用特定物料的所有BOM明细项
        /// </summary>
        public async Task<IEnumerable<BOMItem>> GetByMaterialIdAsync(int materialId)
        {
            return await _bomItemRepository.GetByMaterialIdAsync(materialId);
        }

        /// <summary>
        /// 批量添加BOM明细项
        /// </summary>
        public async Task<IEnumerable<BOMItem>> AddRangeAsync(IEnumerable<BOMItem> items)
        {
            var result = new List<BOMItem>();
            foreach (var item in items)
            {
                result.Add(await AddAsync(item));
            }
            return result;
        }

        /// <summary>
        /// 删除指定BOM的所有明细项
        /// </summary>
        public async Task DeleteByBomIdAsync(int bomId)
        {
            var items = await GetByBomIdAsync(bomId);
            foreach (var item in items)
            {
                await DeleteAsync(item);
            }
        }

        /// <summary>
        /// 更新BOM明细项的关键物料状态
        /// </summary>
        public async Task<BOMItem> UpdateKeyStatusAsync(int itemId, bool isKey)
        {
            var item = await GetByIdAsync(itemId);
            if (item == null)
            {
                throw new ArgumentException($"BOM明细项ID {itemId} 不存在");
            }
            
            item.IsKey = isKey;
            return await UpdateAsync(item);
        }
    }
}