using MES_WPF.Model.BasicInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工序服务接口
    /// </summary>
    public interface IOperationService : IService<Operation>
    {
        /// <summary>
        /// 根据工序编码获取工序
        /// </summary>
        Task<Operation> GetByCodeAsync(string operationCode);
        
        /// <summary>
        /// 根据工序类型获取工序列表
        /// </summary>
        Task<IEnumerable<Operation>> GetByOperationTypeAsync(byte operationType);
        
        /// <summary>
        /// 根据部门获取工序列表
        /// </summary>
        Task<IEnumerable<Operation>> GetByDepartmentAsync(string department);
        
        /// <summary>
        /// 获取有效的工序列表
        /// </summary>
        Task<IEnumerable<Operation>> GetActiveOperationsAsync();

        /// <summary>
        /// 启用/禁用工序
        /// </summary>
        Task<Operation> ToggleOperationStatusAsync(int operationId, bool isActive);

        /// <summary>
        /// 检查工序编码是否存在
        /// </summary>
        Task<bool> IsOperationCodeExistsAsync(string operationCode);
    }
} 