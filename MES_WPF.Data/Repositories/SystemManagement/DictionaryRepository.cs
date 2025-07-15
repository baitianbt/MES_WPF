using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class DictionaryRepository : Repository<Dictionary>, IDictionaryRepository
    {
        public DictionaryRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据字典类型获取字典
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典对象</returns>
        public async Task<Dictionary> GetByTypeAsync(string dictType)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.DictType == dictType);
        }
        
        /// <summary>
        /// 获取字典及其字典项
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>包含字典及字典项的元组</returns>
        public async Task<(Dictionary dict, IEnumerable<DictionaryItem> items)> GetDictionaryWithItemsAsync(string dictType)
        {
            var dict = await GetByTypeAsync(dictType);
            if (dict == null)
            {
                return (null, new List<DictionaryItem>());
            }
            
            var items = await _context.DictionaryItems
                .Where(di => di.DictId == dict.Id && di.Status == 1) // 1表示启用状态
                .OrderBy(di => di.SortOrder)
                .ToListAsync();
            
            return (dict, items);
        }
        
        /// <summary>
        /// 检查字典类型是否已存在
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <param name="excludeId">排除的字典ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsDictTypeExistsAsync(string dictType, int? excludeId = null)
        {
            var query = _dbSet.Where(d => d.DictType == dictType);
            
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
} 