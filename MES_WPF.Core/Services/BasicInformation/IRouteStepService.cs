using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工艺路线步骤服务接口
    /// </summary>
    public interface IRouteStepService : IService<RouteStep>
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

        /// <summary>
        /// 批量添加工艺路线步骤
        /// </summary>
        Task<IEnumerable<RouteStep>> AddRangeAsync(IEnumerable<RouteStep> steps);

        /// <summary>
        /// 删除指定工艺路线的所有步骤
        /// </summary>
        Task DeleteByRouteIdAsync(int routeId);

        /// <summary>
        /// 更新步骤的关键工序状态
        /// </summary>
        Task<RouteStep> UpdateKeyOperationStatusAsync(int stepId, bool isKeyOperation);

        /// <summary>
        /// 更新步骤的质检点状态
        /// </summary>
        Task<RouteStep> UpdateQualityCheckPointStatusAsync(int stepId, bool isQualityCheckPoint);
        
        /// <summary>
        /// 重新排序工艺路线步骤
        /// </summary>
        Task ReorderStepsAsync(int routeId, IEnumerable<int> stepIds);
    }
}