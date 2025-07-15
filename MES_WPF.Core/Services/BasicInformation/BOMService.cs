using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// BOM服务实现
    /// </summary>
    public class BOMService : Service<BOM>, IBOMService
    {
        private readonly IBOMRepository _bomRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BOMService(IBOMRepository bomRepository) : base(bomRepository)
        {
            _bomRepository = bomRepository;
        }

        /// <summary>
        /// 根据BOM编码获取BOM
        /// </summary>
        public async Task<BOM> GetByCodeAsync(string bomCode)
        {
            return await _bomRepository.GetByCodeAsync(bomCode);
        }

        /// <summary>
        /// 获取指定产品的BOM列表
        /// </summary>
        public async Task<IEnumerable<BOM>> GetByProductIdAsync(int productId)
        {
            return await _bomRepository.GetByProductIdAsync(productId);
        }

        /// <summary>
        /// 获取指定产品的默认BOM
        /// </summary>
        public async Task<BOM> GetDefaultByProductIdAsync(int productId)
        {
            return await _bomRepository.GetDefaultByProductIdAsync(productId);
        }

        /// <summary>
        /// 获取指定状态的BOM列表
        /// </summary>
        public async Task<IEnumerable<BOM>> GetByStatusAsync(byte status)
        {
            return await _bomRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// 设置指定产品的默认BOM
        /// </summary>
        public async Task<bool> SetDefaultBOMAsync(int bomId, int productId)
        {
            // 先获取当前产品下的所有BOM
            var boms = (await GetByProductIdAsync(productId)).ToList();
            
            // 检查要设为默认的BOM是否存在
            var targetBom = boms.FirstOrDefault(b => b.Id == bomId);
            if (targetBom == null)
            {
                throw new ArgumentException($"BOM ID {bomId} 不存在或不属于产品 {productId}");
            }
            
            // 取消所有BOM的默认状态
            foreach (var bom in boms.Where(b => b.IsDefault))
            {
                bom.IsDefault = false;
                bom.UpdateTime = DateTime.Now;
                await UpdateAsync(bom);
            }
            
            // 设置目标BOM为默认
            targetBom.IsDefault = true;
            targetBom.UpdateTime = DateTime.Now;
            await UpdateAsync(targetBom);
            
            return true;
        }

        /// <summary>
        /// 更新BOM状态
        /// </summary>
        public async Task<BOM> UpdateStatusAsync(int bomId, byte status)
        {
            var bom = await GetByIdAsync(bomId);
            if (bom == null)
            {
                throw new ArgumentException($"BOM ID {bomId} 不存在");
            }
            
            bom.Status = status;
            bom.UpdateTime = DateTime.Now;
            
            return await UpdateAsync(bom);
        }

        /// <summary>
        /// 检查BOM编码是否存在
        /// </summary>
        public async Task<bool> IsBOMCodeExistsAsync(string bomCode)
        {
            var bom = await GetByCodeAsync(bomCode);
            return bom != null;
        }
    }
}