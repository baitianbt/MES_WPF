using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IDictionaryItemRepository : IRepository<DictionaryItem>
    {
        /// <summary>
        /// 获取指定字典下的字典项
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        Task<IEnumerable<DictionaryItem>> GetItemsByDictIdAsync(int dictId);
        
        /// <summary>
        /// 根据字典类型获取字典项
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项列表</returns>
        Task<IEnumerable<DictionaryItem>> GetItemsByDictTypeAsync(string dictType);
        
        /// <summary>
        /// 获取字典项的值和文本
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项值和文本的键值对</returns>
        Task<IDictionary<string, string>> GetDictItemMapAsync(string dictType);
        
        /// <summary>
        /// 检查指定字典下的字典项值是否已存在
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <param name="itemValue">字典项值</param>
        /// <param name="excludeId">排除的字典项ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsItemValueExistsAsync(int dictId, string itemValue, int? excludeId = null);
    }
} 