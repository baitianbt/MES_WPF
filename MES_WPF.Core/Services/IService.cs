using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services
{
    /// <summary>
    /// 服务层基础接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IService<T> where T : class
    {
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体集合</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// 根据条件查找实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体集合</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体</returns>
        Task<T?> GetByIdAsync(object id);
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>添加后的实体</returns>
        Task<T> AddAsync(T entity);
        
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新后的实体</returns>
        Task<T> UpdateAsync(T entity);
        
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(T entity);
        
        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>任务</returns>
        Task DeleteByIdAsync(object id);
    }
} 