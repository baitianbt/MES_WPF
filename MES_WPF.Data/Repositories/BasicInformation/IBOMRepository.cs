using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IBOMRepository : IRepository<BOM>
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
    }
} 