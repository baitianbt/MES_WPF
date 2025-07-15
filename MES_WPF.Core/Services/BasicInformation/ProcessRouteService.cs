using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工艺路线服务实现
    /// </summary>
    public class ProcessRouteService : Service<ProcessRoute>, IProcessRouteService
    {
        private readonly IProcessRouteRepository _processRouteRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ProcessRouteService(IProcessRouteRepository processRouteRepository) : base(processRouteRepository)
        {
            _processRouteRepository = processRouteRepository;
        }

        /// <summary>
        /// 根据工艺路线编码获取工艺路线
        /// </summary>
        public async Task<ProcessRoute> GetByCodeAsync(string routeCode)
        {
            return await _processRouteRepository.GetByCodeAsync(routeCode);
        }

        /// <summary>
        /// 获取指定产品的工艺路线列表
        /// </summary>
        public async Task<IEnumerable<ProcessRoute>> GetByProductIdAsync(int productId)
        {
            return await _processRouteRepository.GetByProductIdAsync(productId);
        }

        /// <summary>
        /// 获取指定产品的默认工艺路线
        /// </summary>
        public async Task<ProcessRoute> GetDefaultByProductIdAsync(int productId)
        {
            return await _processRouteRepository.GetDefaultByProductIdAsync(productId);
        }

        /// <summary>
        /// 获取指定状态的工艺路线列表
        /// </summary>
        public async Task<IEnumerable<ProcessRoute>> GetByStatusAsync(byte status)
        {
            return await _processRouteRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// 设置指定产品的默认工艺路线
        /// </summary>
        public async Task<bool> SetDefaultRouteAsync(int routeId, int productId)
        {
            // 先获取当前产品下的所有工艺路线
            var routes = (await GetByProductIdAsync(productId)).ToList();
            
            // 检查要设为默认的工艺路线是否存在
            var targetRoute = routes.FirstOrDefault(r => r.Id == routeId);
            if (targetRoute == null)
            {
                throw new ArgumentException($"工艺路线ID {routeId} 不存在或不属于产品 {productId}");
            }
            
            // 取消所有工艺路线的默认状态
            foreach (var route in routes.Where(r => r.IsDefault))
            {
                route.IsDefault = false;
                route.UpdateTime = DateTime.Now;
                await UpdateAsync(route);
            }
            
            // 设置目标工艺路线为默认
            targetRoute.IsDefault = true;
            targetRoute.UpdateTime = DateTime.Now;
            await UpdateAsync(targetRoute);
            
            return true;
        }

        /// <summary>
        /// 更新工艺路线状态
        /// </summary>
        public async Task<ProcessRoute> UpdateStatusAsync(int routeId, byte status)
        {
            var route = await GetByIdAsync(routeId);
            if (route == null)
            {
                throw new ArgumentException($"工艺路线ID {routeId} 不存在");
            }
            
            route.Status = status;
            route.UpdateTime = DateTime.Now;
            
            return await UpdateAsync(route);
        }

        /// <summary>
        /// 检查工艺路线编码是否存在
        /// </summary>
        public async Task<bool> IsRouteCodeExistsAsync(string routeCode)
        {
            var route = await GetByCodeAsync(routeCode);
            return route != null;
        }
    }
}