using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// BOM服务接口
    /// </summary>
    public interface IBOMService : IService<BOM>
    {
        /// <summary>
        /// 根据BOM编码获取BOM
        /// </summary>
        Task<BOM> GetByCodeAsync(string bomCode);
        
        /// <summary>
        /// 获取指定产品的BOM列表
        /// </summary>
        Task<IEnumerable<BOM>> GetByProductIdAsync(int productId);
        
        /// <summary>
        /// 获取指定产品的默认BOM
        /// </summary>
        Task<BOM> GetDefaultByProductIdAsync(int productId);
        
        /// <summary>
        /// 获取指定状态的BOM列表
        /// </summary>
        Task<IEnumerable<BOM>> GetByStatusAsync(byte status);

        /// <summary>
        /// 设置指定产品的默认BOM
        /// </summary>
        Task<bool> SetDefaultBOMAsync(int bomId, int productId);

        /// <summary>
        /// 更新BOM状态
        /// </summary>
        Task<BOM> UpdateStatusAsync(int bomId, byte status);

        /// <summary>
        /// 检查BOM编码是否存在
        /// </summary>
        Task<bool> IsBOMCodeExistsAsync(string bomCode);
    }
}