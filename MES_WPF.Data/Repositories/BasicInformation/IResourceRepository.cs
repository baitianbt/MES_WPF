using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public interface IResourceRepository : IRepository<Resource>
    {
        /// <summary>
        /// 根据资源编码获取资源
        /// </summary>
        Task<Resource> GetByCodeAsync(string resourceCode);
        
        /// <summary>
        /// 根据资源类型获取资源列表
        /// </summary>
        Task<IEnumerable<Resource>> GetByResourceTypeAsync(byte resourceType);
        
        /// <summary>
        /// 根据部门ID获取资源列表
        /// </summary>
        Task<IEnumerable<Resource>> GetByDepartmentIdAsync(int departmentId);
        
        /// <summary>
        /// 根据状态获取资源列表
        /// </summary>
        Task<IEnumerable<Resource>> GetByStatusAsync(byte status);
        
        /// <summary>
        /// 获取所有设备类型资源（带设备详情）
        /// </summary>
        Task<IEnumerable<Resource>> GetAllEquipmentResourcesAsync();
    }
} 