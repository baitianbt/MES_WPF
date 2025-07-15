using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class ProcessRouteRepository : Repository<ProcessRoute>, IProcessRouteRepository
    {
        public ProcessRouteRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据工艺路线编码获取工艺路线
        /// </summary>
        public async Task<ProcessRoute> GetByCodeAsync(string routeCode)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.RouteCode == routeCode);
        }

        /// <summary>
        /// 获取指定产品的工艺路线列表
        /// </summary>
        public async Task<IEnumerable<ProcessRoute>> GetByProductIdAsync(int productId)
        {
            return await _dbSet.Where(r => r.ProductId == productId).ToListAsync();
        }

        /// <summary>
        /// 获取指定产品的默认工艺路线
        /// </summary>
        public async Task<ProcessRoute> GetDefaultByProductIdAsync(int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.ProductId == productId && r.IsDefault);
        }

        /// <summary>
        /// 获取指定状态的工艺路线列表
        /// </summary>
        public async Task<IEnumerable<ProcessRoute>> GetByStatusAsync(byte status)
        {
            return await _dbSet.Where(r => r.Status == status).ToListAsync();
        }
    }
}