using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.EquipmentManagement
{
    public class SpareRepository : Repository<Spare>, ISpareRepository
    {
        public SpareRepository(MesDbContext context) : base(context)
        {
        }
        
        /// <summary>
        /// 根据备件编码获取备件
        /// </summary>
        /// <param name="spareCode">备件编码</param>
        /// <returns>备件</returns>
        public async Task<Spare> GetBySpareCodeAsync(string spareCode)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.SpareCode == spareCode);
        }
        
        /// <summary>
        /// 根据备件类型获取备件列表
        /// </summary>
        /// <param name="spareType">备件类型</param>
        /// <returns>备件列表</returns>
        public async Task<IEnumerable<Spare>> GetBySpareTypeAsync(byte spareType)
        {
            return await _dbSet.Where(s => s.SpareType == spareType).ToListAsync();
        }
        
        /// <summary>
        /// 获取库存低于最低库存的备件列表
        /// </summary>
        /// <returns>库存不足的备件列表</returns>
        public async Task<IEnumerable<Spare>> GetLowStockSparesAsync()
        {
            return await _dbSet
                .Where(s => s.StockQuantity < s.MinimumStock && s.IsActive)
                .OrderBy(s => s.StockQuantity / s.MinimumStock)
                .ToListAsync();
        }
        
        /// <summary>
        /// 更新备件库存
        /// </summary>
        /// <param name="id">备件ID</param>
        /// <param name="quantity">数量变化（正数为增加，负数为减少）</param>
        /// <returns>更新后的备件</returns>
        public async Task<Spare> UpdateStockQuantityAsync(int id, decimal quantity)
        {
            var spare = await _dbSet.FindAsync(id);
            if (spare == null)
                throw new ArgumentException($"找不到ID为{id}的备件", nameof(id));
            
            // 检查库存是否足够（如果是减少库存）
            if (quantity < 0 && spare.StockQuantity < Math.Abs(quantity))
                throw new InvalidOperationException($"备件{spare.SpareName}库存不足，当前库存: {spare.StockQuantity}，需要: {Math.Abs(quantity)}");
            
            spare.StockQuantity += quantity;
            spare.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return spare;
        }
        
        /// <summary>
        /// 启用/禁用备件
        /// </summary>
        /// <param name="id">备件ID</param>
        /// <param name="isActive">是否有效</param>
        /// <returns>更新后的备件</returns>
        public async Task<Spare> SetActiveStatusAsync(int id, bool isActive)
        {
            var spare = await _dbSet.FindAsync(id);
            if (spare == null)
                throw new ArgumentException($"找不到ID为{id}的备件", nameof(id));
            
            spare.IsActive = isActive;
            spare.UpdateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            
            return spare;
        }
        
        /// <summary>
        /// 根据关键字搜索备件
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>备件列表</returns>
        public async Task<IEnumerable<Spare>> SearchSparesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _dbSet.Where(s => s.IsActive).ToListAsync();
                
            keyword = keyword.ToLower();
            
            return await _dbSet
                .Where(s => s.IsActive &&
                       (s.SpareCode.ToLower().Contains(keyword) ||
                        s.SpareName.ToLower().Contains(keyword) ||
                        s.Specification != null && s.Specification.ToLower().Contains(keyword) ||
                        s.Supplier != null && s.Supplier.ToLower().Contains(keyword)))
                .ToListAsync();
        }
    }
} 