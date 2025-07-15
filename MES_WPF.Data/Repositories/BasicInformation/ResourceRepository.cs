using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据资源编码获取资源
        /// </summary>
        public async Task<Resource> GetByCodeAsync(string resourceCode)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.ResourceCode == resourceCode);
        }

        /// <summary>
        /// 根据资源类型获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByResourceTypeAsync(byte resourceType)
        {
            return await _dbSet.Where(r => r.ResourceType == resourceType).ToListAsync();
        }

        /// <summary>
        /// 根据部门ID获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _dbSet.Where(r => r.DepartmentId == departmentId).ToListAsync();
        }

        /// <summary>
        /// 根据状态获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByStatusAsync(byte status)
        {
            return await _dbSet.Where(r => r.Status == status).ToListAsync();
        }

        /// <summary>
        /// 获取所有设备类型资源（带设备详情）
        /// </summary>
        public async Task<IEnumerable<Resource>> GetAllEquipmentResourcesAsync()
        {
            // 设备类型为1
            return await _dbSet.Where(r => r.ResourceType == 1)
                              .Include(r => r.Equipment)
                              .ToListAsync();
        }
    }
} 