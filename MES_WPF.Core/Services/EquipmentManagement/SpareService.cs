using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 备件服务实现
    /// </summary>
    public class SpareService : Service<Spare>, ISpareService
    {
        private readonly ISpareRepository _spareRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">备件仓储</param>
        public SpareService(ISpareRepository repository) : base(repository)
        {
            _spareRepository = repository;
        }
        
        /// <summary>
        /// 根据备件编码获取备件
        /// </summary>
        /// <param name="spareCode">备件编码</param>
        /// <returns>备件</returns>
        public async Task<Spare> GetBySpareCodeAsync(string spareCode)
        {
            return await _spareRepository.GetBySpareCodeAsync(spareCode);
        }
        
        /// <summary>
        /// 根据备件类型获取备件列表
        /// </summary>
        /// <param name="spareType">备件类型</param>
        /// <returns>备件列表</returns>
        public async Task<IEnumerable<Spare>> GetBySpareTypeAsync(byte spareType)
        {
            return await _spareRepository.GetBySpareTypeAsync(spareType);
        }
        
        /// <summary>
        /// 获取库存低于最低库存的备件列表
        /// </summary>
        /// <returns>库存不足的备件列表</returns>
        public async Task<IEnumerable<Spare>> GetLowStockSparesAsync()
        {
            return await _spareRepository.GetLowStockSparesAsync();
        }
        
        /// <summary>
        /// 更新备件库存
        /// </summary>
        /// <param name="id">备件ID</param>
        /// <param name="quantity">数量变化（正数为增加，负数为减少）</param>
        /// <returns>更新后的备件</returns>
        public async Task<Spare> UpdateStockQuantityAsync(int id, decimal quantity)
        {
            return await _spareRepository.UpdateStockQuantityAsync(id, quantity);
        }
        
        /// <summary>
        /// 启用/禁用备件
        /// </summary>
        /// <param name="id">备件ID</param>
        /// <param name="isActive">是否有效</param>
        /// <returns>更新后的备件</returns>
        public async Task<Spare> SetActiveStatusAsync(int id, bool isActive)
        {
            return await _spareRepository.SetActiveStatusAsync(id, isActive);
        }
        
        /// <summary>
        /// 根据关键字搜索备件
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>备件列表</returns>
        public async Task<IEnumerable<Spare>> SearchSparesAsync(string keyword)
        {
            return await _spareRepository.SearchSparesAsync(keyword);
        }
    }
} 