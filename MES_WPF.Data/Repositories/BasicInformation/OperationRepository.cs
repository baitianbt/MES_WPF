using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class OperationRepository : Repository<Operation>, IOperationRepository
    {
        public OperationRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据工序编码获取工序
        /// </summary>
        public async Task<Operation> GetByCodeAsync(string operationCode)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.OperationCode == operationCode);
        }

        /// <summary>
        /// 根据工序类型获取工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetByOperationTypeAsync(byte operationType)
        {
            return await _dbSet.Where(o => o.OperationType == operationType).ToListAsync();
        }

        /// <summary>
        /// 根据部门获取工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetByDepartmentAsync(string department)
        {
            return await _dbSet.Where(o => o.Department == department).ToListAsync();
        }

        /// <summary>
        /// 获取有效的工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetActiveOperationsAsync()
        {
            return await _dbSet.Where(o => o.IsActive).ToListAsync();
        }
    }
} 