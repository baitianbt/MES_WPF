using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 工序服务实现
    /// </summary>
    public class OperationService : Service<Operation>, IOperationService
    {
        private readonly IOperationRepository _operationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OperationService(IOperationRepository operationRepository) : base(operationRepository)
        {
            _operationRepository = operationRepository;
        }

        /// <summary>
        /// 根据工序编码获取工序
        /// </summary>
        public async Task<Operation> GetByCodeAsync(string operationCode)
        {
            return await _operationRepository.GetByCodeAsync(operationCode);
        }

        /// <summary>
        /// 根据工序类型获取工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetByOperationTypeAsync(byte operationType)
        {
            return await _operationRepository.GetByOperationTypeAsync(operationType);
        }

        /// <summary>
        /// 根据部门获取工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetByDepartmentAsync(string department)
        {
            return await _operationRepository.GetByDepartmentAsync(department);
        }

        /// <summary>
        /// 获取有效的工序列表
        /// </summary>
        public async Task<IEnumerable<Operation>> GetActiveOperationsAsync()
        {
            return await _operationRepository.GetActiveOperationsAsync();
        }

        /// <summary>
        /// 启用/禁用工序
        /// </summary>
        public async Task<Operation> ToggleOperationStatusAsync(int operationId, bool isActive)
        {
            var operation = await GetByIdAsync(operationId);
            if (operation == null)
            {
                throw new ArgumentException($"工序ID {operationId} 不存在");
            }

            operation.IsActive = isActive;
            operation.UpdateTime = DateTime.Now;
            
            return await UpdateAsync(operation);
        }

        /// <summary>
        /// 检查工序编码是否存在
        /// </summary>
        public async Task<bool> IsOperationCodeExistsAsync(string operationCode)
        {
            var operation = await GetByCodeAsync(operationCode);
            return operation != null;
        }
    }
} 