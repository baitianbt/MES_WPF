using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工艺路线步骤服务实现
    /// </summary>
    public class RouteStepService : Service<RouteStep>, IRouteStepService
    {
        private readonly IRouteStepRepository _routeStepRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RouteStepService(IRouteStepRepository routeStepRepository) : base(routeStepRepository)
        {
            _routeStepRepository = routeStepRepository;
        }

        /// <summary>
        /// 获取指定工艺路线的所有步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetByRouteIdAsync(int routeId)
        {
            return await _routeStepRepository.GetByRouteIdAsync(routeId);
        }

        /// <summary>
        /// 获取指定工艺路线的关键工序步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetKeyStepsByRouteIdAsync(int routeId)
        {
            return await _routeStepRepository.GetKeyStepsByRouteIdAsync(routeId);
        }

        /// <summary>
        /// 获取指定工艺路线的质检点步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetQualityCheckPointsByRouteIdAsync(int routeId)
        {
            return await _routeStepRepository.GetQualityCheckPointsByRouteIdAsync(routeId);
        }

        /// <summary>
        /// 获取使用特定工序的所有步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetByOperationIdAsync(int operationId)
        {
            return await _routeStepRepository.GetByOperationIdAsync(operationId);
        }

        /// <summary>
        /// 批量添加工艺路线步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> AddRangeAsync(IEnumerable<RouteStep> steps)
        {
            var result = new List<RouteStep>();
            foreach (var step in steps)
            {
                result.Add(await AddAsync(step));
            }
            return result;
        }

        /// <summary>
        /// 删除指定工艺路线的所有步骤
        /// </summary>
        public async Task DeleteByRouteIdAsync(int routeId)
        {
            var steps = await GetByRouteIdAsync(routeId);
            foreach (var step in steps)
            {
                await DeleteAsync(step);
            }
        }

        /// <summary>
        /// 更新步骤的关键工序状态
        /// </summary>
        public async Task<RouteStep> UpdateKeyOperationStatusAsync(int stepId, bool isKeyOperation)
        {
            var step = await GetByIdAsync(stepId);
            if (step == null)
            {
                throw new ArgumentException($"工艺步骤ID {stepId} 不存在");
            }
            
            step.IsKeyOperation = isKeyOperation;
            return await UpdateAsync(step);
        }

        /// <summary>
        /// 更新步骤的质检点状态
        /// </summary>
        public async Task<RouteStep> UpdateQualityCheckPointStatusAsync(int stepId, bool isQualityCheckPoint)
        {
            var step = await GetByIdAsync(stepId);
            if (step == null)
            {
                throw new ArgumentException($"工艺步骤ID {stepId} 不存在");
            }
            
            step.IsQualityCheckPoint = isQualityCheckPoint;
            return await UpdateAsync(step);
        }
        
        /// <summary>
        /// 重新排序工艺路线步骤
        /// </summary>
        public async Task ReorderStepsAsync(int routeId, IEnumerable<int> stepIds)
        {
            var steps = (await GetByRouteIdAsync(routeId)).ToDictionary(s => s.Id);
            var stepIdsList = stepIds.ToList();
            
            // 验证所有步骤ID是否都存在
            foreach (var stepId in stepIdsList)
            {
                if (!steps.ContainsKey(stepId))
                {
                    throw new ArgumentException($"步骤ID {stepId} 不存在或不属于工艺路线 {routeId}");
                }
            }
            
            // 重新排序
            for (int i = 0; i < stepIdsList.Count; i++)
            {
                var step = steps[stepIdsList[i]];
                step.StepNo = i + 1;
                await UpdateAsync(step);
            }
        }
    }
} 