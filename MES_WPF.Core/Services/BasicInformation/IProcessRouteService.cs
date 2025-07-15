using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工艺路线服务接口
    /// </summary>
    public interface IProcessRouteService : IService<ProcessRoute>
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

        /// <summary>
        /// 设置指定产品的默认工艺路线
        /// </summary>
        Task<bool> SetDefaultRouteAsync(int routeId, int productId);

        /// <summary>
        /// 更新工艺路线状态
        /// </summary>
        Task<ProcessRoute> UpdateStatusAsync(int routeId, byte status);

        /// <summary>
        /// 检查工艺路线编码是否存在
        /// </summary>
        Task<bool> IsRouteCodeExistsAsync(string routeCode);
    }
}