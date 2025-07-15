using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IRouteStepRepository : IRepository<RouteStep>
    {
        /// <summary>
        /// 获取指定工艺路线的所有步骤
        /// </summary>
        Task<IEnumerable<RouteStep>> GetByRouteIdAsync(int routeId);
        
        /// <summary>
        /// 获取指定工艺路线的关键工序步骤
        /// </summary>
        Task<IEnumerable<RouteStep>> GetKeyStepsByRouteIdAsync(int routeId);
        
        /// <summary>
        /// 获取指定工艺路线的质检点步骤
        /// </summary>
        Task<IEnumerable<RouteStep>> GetQualityCheckPointsByRouteIdAsync(int routeId);
        
        /// <summary>
        /// 获取使用特定工序的所有步骤
        /// </summary>
        Task<IEnumerable<RouteStep>> GetByOperationIdAsync(int operationId);
    }
} 