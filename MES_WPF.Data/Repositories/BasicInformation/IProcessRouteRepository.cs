using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IProcessRouteRepository : IRepository<ProcessRoute>
    {
        /// <summary>
        /// 根据工艺路线编码获取工艺路线
        /// </summary>
        Task<ProcessRoute> GetByCodeAsync(string routeCode);
        
        /// <summary>
        /// 获取指定产品的工艺路线列表
        /// </summary>
        Task<IEnumerable<ProcessRoute>> GetByProductIdAsync(int productId);
        
        /// <summary>
        /// 获取指定产品的默认工艺路线
        /// </summary>
        Task<ProcessRoute> GetDefaultByProductIdAsync(int productId);
        
        /// <summary>
        /// 获取指定状态的工艺路线列表
        /// </summary>
        Task<IEnumerable<ProcessRoute>> GetByStatusAsync(byte status);
    }
} 