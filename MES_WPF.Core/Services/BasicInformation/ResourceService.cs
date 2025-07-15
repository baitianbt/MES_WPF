using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 资源服务实现
    /// </summary>
    public class ResourceService : Service<Resource>, IResourceService
    {
        private readonly IResourceRepository _resourceRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceService(IResourceRepository resourceRepository) : base(resourceRepository)
        {
            _resourceRepository = resourceRepository;
        }

        /// <summary>
        /// 根据资源编码获取资源
        /// </summary>
        public async Task<Resource> GetByCodeAsync(string resourceCode)
        {
            return await _resourceRepository.GetByCodeAsync(resourceCode);
        }

        /// <summary>
        /// 根据资源类型获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByResourceTypeAsync(byte resourceType)
        {
            return await _resourceRepository.GetByResourceTypeAsync(resourceType);
        }

        /// <summary>
        /// 根据部门ID获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _resourceRepository.GetByDepartmentIdAsync(departmentId);
        }

        /// <summary>
        /// 根据状态获取资源列表
        /// </summary>
        public async Task<IEnumerable<Resource>> GetByStatusAsync(byte status)
        {
            return await _resourceRepository.GetByStatusAsync(status);
        }

        /// <summary>
        /// 获取所有设备类型资源（带设备详情）
        /// </summary>
        public async Task<IEnumerable<Resource>> GetAllEquipmentResourcesAsync()
        {
            return await _resourceRepository.GetAllEquipmentResourcesAsync();
        }

        /// <summary>
        /// 更新资源状态
        /// </summary>
        public async Task<Resource> UpdateStatusAsync(int resourceId, byte status)
        {
            var resource = await GetByIdAsync(resourceId);
            if (resource == null)
            {
                throw new ArgumentException($"资源ID {resourceId} 不存在");
            }
            
            resource.Status = status;
            resource.UpdateTime = DateTime.Now;
            
            return await UpdateAsync(resource);
        }

        /// <summary>
        /// 检查资源编码是否存在
        /// </summary>
        public async Task<bool> IsResourceCodeExistsAsync(string resourceCode)
        {
            var resource = await GetByCodeAsync(resourceCode);
            return resource != null;
        }
    }
}