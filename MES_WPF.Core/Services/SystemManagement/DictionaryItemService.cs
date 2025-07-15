using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 字典项服务实现
    /// </summary>
    public class DictionaryItemService : Service<DictionaryItem>, IDictionaryItemService
    {
        private readonly IDictionaryItemRepository _dictionaryItemRepository;
        
        public DictionaryItemService(IDictionaryItemRepository dictionaryItemRepository) 
            : base(dictionaryItemRepository)
        {
            _dictionaryItemRepository = dictionaryItemRepository ?? throw new ArgumentNullException(nameof(dictionaryItemRepository));
        }
        
        /// <summary>
        /// 根据字典ID获取字典项
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetByDictIdAsync(int dictId)
        {
            return await _dictionaryItemRepository.GetItemsByDictIdAsync(dictId);
        }
        
        /// <summary>
        /// 根据字典类型获取字典项
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetByDictTypeAsync(string dictType)
        {
            return await _dictionaryItemRepository.GetItemsByDictTypeAsync(dictType);
        }
        
        /// <summary>
        /// 获取字典项的值和文本映射
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项值和文本的键值对</returns>
        public async Task<IDictionary<string, string>> GetDictItemMapAsync(string dictType)
        {
            return await _dictionaryItemRepository.GetDictItemMapAsync(dictType);
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
            return await _dictionaryItemRepository.IsItemValueExistsAsync(dictId, itemValue, excludeId);
        }
        
        /// <summary>
        /// 批量删除字典项
        /// </summary>
        /// <param name="ids">字典项ID集合</param>
        /// <returns>是否成功</returns>
        public async Task<bool> BatchDeleteAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return false;
            }
            
            try
            {
                foreach (var id in ids)
                {
                    await _dictionaryItemRepository.DeleteByIdAsync(id);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 