using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 数据字典服务实现
    /// </summary>
    public class DictionaryService : Service<Dictionary>, IDictionaryService
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IDictionaryItemRepository _dictionaryItemRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dictionaryRepository">字典仓储</param>
        /// <param name="dictionaryItemRepository">字典项仓储</param>
        public DictionaryService(
            IDictionaryRepository dictionaryRepository,
            IDictionaryItemRepository dictionaryItemRepository) 
            : base(dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository ?? throw new ArgumentNullException(nameof(dictionaryRepository));
            _dictionaryItemRepository = dictionaryItemRepository ?? throw new ArgumentNullException(nameof(dictionaryItemRepository));
        }

        /// <summary>
        /// 根据字典类型获取字典
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典</returns>
        public async Task<Dictionary> GetByDictTypeAsync(string dictType)
        {
            return await _dictionaryRepository.GetByTypeAsync(dictType);
        }
        
        /// <summary>
        /// 获取字典项列表
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetDictItemsAsync(int dictId)
        {
            return await _dictionaryItemRepository.GetItemsByDictIdAsync(dictId);
        }
        
        /// <summary>
        /// 根据字典类型获取字典项列表
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项列表</returns>
        public async Task<IEnumerable<DictionaryItem>> GetDictItemsByTypeAsync(string dictType)
        {
            var dictionary = await GetByDictTypeAsync(dictType);
            if (dictionary == null)
            {
                return new List<DictionaryItem>();
            }
            
            return await GetDictItemsAsync(dictionary.Id);
        }
        
        /// <summary>
        /// 添加字典项
        /// </summary>
        /// <param name="dictItem">字典项</param>
        /// <returns>添加的字典项</returns>
        public async Task<DictionaryItem> AddDictItemAsync(DictionaryItem dictItem)
        {
            dictItem.CreateTime = DateTime.Now;
            return await _dictionaryItemRepository.AddAsync(dictItem);
        }
        
        /// <summary>
        /// 更新字典项
        /// </summary>
        /// <param name="dictItem">字典项</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDictItemAsync(DictionaryItem dictItem)
        {
            try
            {
                dictItem.UpdateTime = DateTime.Now;
                await _dictionaryItemRepository.UpdateAsync(dictItem);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="dictItemId">字典项ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDictItemAsync(int dictItemId)
        {
            try
            {
                await _dictionaryItemRepository.DeleteByIdAsync(dictItemId);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 批量删除字典项
        /// </summary>
        /// <param name="dictItemIds">字典项ID集合</param>
        /// <returns>是否成功</returns>
        public async Task<bool> BatchDeleteDictItemsAsync(IEnumerable<int> dictItemIds)
        {
            try
            {
                foreach (var id in dictItemIds)
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