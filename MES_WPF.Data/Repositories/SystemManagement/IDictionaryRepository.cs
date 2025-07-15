using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IDictionaryRepository : IRepository<Dictionary>
    {
        /// <summary>
        /// 根据字典类型获取字典
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>字典对象</returns>
        Task<Dictionary> GetByTypeAsync(string dictType);
        
        /// <summary>
        /// 获取字典及其字典项
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <returns>包含字典及字典项的元组</returns>
        Task<(Dictionary dict, IEnumerable<DictionaryItem> items)> GetDictionaryWithItemsAsync(string dictType);
        
        /// <summary>
        /// 检查字典类型是否已存在
        /// </summary>
        /// <param name="dictType">字典类型</param>
        /// <param name="excludeId">排除的字典ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsDictTypeExistsAsync(string dictType, int? excludeId = null);
    }
} 