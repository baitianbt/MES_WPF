using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class DictionaryItemRepository : Repository<DictionaryItem>, IDictionaryItemRepository
    {
        public DictionaryItemRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取指定字典下的字典项
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetItemsByDictIdAsync(int dictId)
        {
            return await _dbSet
                .Where(di => di.DictId == dictId && di.Status == 1) // 1表示启用状态
                .OrderBy(di => di.SortOrder)
                .ToListAsync();
        }
        
        /// <summary>
        /// 根据字典类型获取字典项
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetItemsByDictTypeAsync(string dictType)
        {
            // 先获取字典
            var dictionary = await _context.Dictionaries
                .FirstOrDefaultAsync(d => d.DictType == dictType && d.Status == 1);
            
            if (dictionary == null)
            {
                return new List<DictionaryItem>();
            }
            
            // 再获取字典项
            return await GetItemsByDictIdAsync(dictionary.Id);
        }
        
        /// <summary>
        /// 获取字典项的值和文本
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项值和文本的键值对</returns>
        public async Task<IDictionary<string, string>> GetDictItemMapAsync(string dictType)
        {
            var items = await GetItemsByDictTypeAsync(dictType);
            return items.ToDictionary(i => i.ItemValue, i => i.ItemText);
        }
        
        /// <summary>
        /// 检查指定字典下的字典项值是否已存在
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <param name="itemValue">字典项值</param>
        /// <param name="excludeId">排除的字典项ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsItemValueExistsAsync(int dictId, string itemValue, int? excludeId = null)
        {
            var query = _dbSet.Where(di => di.DictId == dictId && di.ItemValue == itemValue);
            
            if (excludeId.HasValue)
            {
                query = query.Where(di => di.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
} 