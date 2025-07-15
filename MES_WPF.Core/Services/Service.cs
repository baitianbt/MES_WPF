using MES_WPF.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services
{
    /// <summary>
    /// 基于仓储层的服务基类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class Service<T> : IService<T> where T : class
    {
        /// <summary>
        /// 仓储对象
        /// </summary>
        protected readonly IRepository<T> _repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">仓储对象</param>
        public Service(IRepository<T> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体集合</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// 根据条件查找实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体集合</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _repository.FindAsync(predicate);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体</returns>
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>添加后的实体</returns>
        public async Task<T> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新后的实体</returns>
        public async Task<T> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(T entity)
        {
            await _repository.DeleteAsync(entity);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>任务</returns>
        public async Task DeleteByIdAsync(object id)
        {
            await _repository.DeleteByIdAsync(id);
        }
    }
}