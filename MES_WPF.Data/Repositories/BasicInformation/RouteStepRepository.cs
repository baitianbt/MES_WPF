using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class RouteStepRepository : Repository<RouteStep>, IRouteStepRepository
    {
        public RouteStepRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 获取指定工艺路线的所有步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetByRouteIdAsync(int routeId)
        {
            return await _dbSet.Where(s => s.RouteId == routeId)
                              .Include(s => s.Operation)
                              .OrderBy(s => s.StepNo)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取指定工艺路线的关键工序步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetKeyStepsByRouteIdAsync(int routeId)
        {
            return await _dbSet.Where(s => s.RouteId == routeId && s.IsKeyOperation)
                              .Include(s => s.Operation)
                              .OrderBy(s => s.StepNo)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取指定工艺路线的质检点步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetQualityCheckPointsByRouteIdAsync(int routeId)
        {
            return await _dbSet.Where(s => s.RouteId == routeId && s.IsQualityCheckPoint)
                              .Include(s => s.Operation)
                              .OrderBy(s => s.StepNo)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取使用特定工序的所有步骤
        /// </summary>
        public async Task<IEnumerable<RouteStep>> GetByOperationIdAsync(int operationId)
        {
            return await _dbSet.Where(s => s.OperationId == operationId)
                              .Include(s => s.ProcessRoute)
                              .ToListAsync();
        }
    }
} 