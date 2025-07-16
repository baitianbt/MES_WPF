using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.EquipmentManagement
{
    /// <summary>
    /// 设备维护计划服务实现
    /// </summary>
    public class EquipmentMaintenancePlanService : Service<EquipmentMaintenancePlan>, IEquipmentMaintenancePlanService
    {
        private readonly IEquipmentMaintenancePlanRepository _equipmentMaintenancePlanRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">维护计划仓储</param>
        public EquipmentMaintenancePlanService(IEquipmentMaintenancePlanRepository repository) : base(repository)
        {
            _equipmentMaintenancePlanRepository = repository;
        }
        
        /// <summary>
        /// 获取特定设备的维护计划列表
        /// </summary>
        /// <param name="equipmentId">设备ID</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetByEquipmentIdAsync(int equipmentId)
        {
            return await _equipmentMaintenancePlanRepository.GetByEquipmentIdAsync(equipmentId);
        }
        
        /// <summary>
        /// 获取需要执行的维护计划（下次执行日期小于等于指定日期）
        /// </summary>
        /// <param name="date">指定日期</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetPlansForExecutionAsync(DateTime date)
        {
            return await _equipmentMaintenancePlanRepository.GetPlansForExecutionAsync(date);
        }
        
        /// <summary>
        /// 根据维护类型获取维护计划
        /// </summary>
        /// <param name="maintenanceType">维护类型</param>
        /// <returns>维护计划列表</returns>
        public async Task<IEnumerable<EquipmentMaintenancePlan>> GetByMaintenanceTypeAsync(byte maintenanceType)
        {
            return await _equipmentMaintenancePlanRepository.GetByMaintenanceTypeAsync(maintenanceType);
        }
        
        /// <summary>
        /// 根据计划编码获取维护计划
        /// </summary>
        /// <param name="planCode">计划编码</param>
        /// <returns>维护计划</returns>
        public async Task<EquipmentMaintenancePlan> GetByPlanCodeAsync(string planCode)
        {
            return await _equipmentMaintenancePlanRepository.GetByPlanCodeAsync(planCode);
        }
        
        /// <summary>
        /// 更新计划的下一次执行日期
        /// </summary>
        /// <param name="id">维护计划ID</param>
        /// <param name="lastExecuteDate">上次执行日期</param>
        /// <returns>更新后的维护计划</returns>
        public async Task<EquipmentMaintenancePlan> UpdateExecuteDateAsync(int id, DateTime lastExecuteDate)
        {
            return await _equipmentMaintenancePlanRepository.UpdateExecuteDateAsync(id, lastExecuteDate);
        }
    }
}