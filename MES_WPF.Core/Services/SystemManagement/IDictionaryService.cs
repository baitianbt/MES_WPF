using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 数据字典服务接口
    /// </summary>
    public interface IDictionaryService : IService<Dictionary>
    {
        /// <summary>
        /// 根据字典类型获取字典
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典</returns>
        Task<Dictionary> GetByDictTypeAsync(string dictType);
        
        /// <summary>
        /// 获取字典项列表
        /// </summary>
        /// <param name="dictId">字典ID</param>
        /// <returns>字典项列表</returns>
        Task<IEnumerable<DictionaryItem>> GetDictItemsAsync(int dictId);
        
        /// <summary>
        /// 根据字典类型获取字典项列表
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典项列表</returns>
        Task<IEnumerable<DictionaryItem>> GetDictItemsByTypeAsync(string dictType);
        
        /// <summary>
        /// 添加字典项
        /// </summary>
        /// <param name="dictItem">字典项</param>
        /// <returns>添加的字典项</returns>
        Task<DictionaryItem> AddDictItemAsync(DictionaryItem dictItem);
        
        /// <summary>
        /// 更新字典项
        /// </summary>
        /// <param name="dictItem">字典项</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateDictItemAsync(DictionaryItem dictItem);
        
        /// <summary>
        /// 删除字典项
        /// </summary>
        /// <param name="dictItemId">字典项ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteDictItemAsync(int dictItemId);
        
        /// <summary>
        /// 批量删除字典项
        /// </summary>
        /// <param name="dictItemIds">字典项ID集合</param>
        /// <returns>是否成功</returns>
        Task<bool> BatchDeleteDictItemsAsync(IEnumerable<int> dictItemIds);
    }
} 