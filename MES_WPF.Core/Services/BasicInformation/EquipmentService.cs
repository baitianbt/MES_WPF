using MES_WPF.Data.Repositories.BasicInformation;
using MES_WPF.Model.BasicInformation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.BasicInformation
{
    /// <summary>
    /// 设备服务实现
    /// </summary>
    public class EquipmentService : Service<Equipment>, IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public EquipmentService(IEquipmentRepository equipmentRepository) : base(equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        /// <summary>
        /// 根据资源ID获取设备
        /// </summary>
        public async Task<Equipment> GetByResourceIdAsync(int resourceId)
        {
            return await _equipmentRepository.GetByResourceIdAsync(resourceId);
        }

        /// <summary>
        /// 根据序列号获取设备
        /// </summary>
        public async Task<Equipment> GetBySerialNumberAsync(string serialNumber)
        {
            return await _equipmentRepository.GetBySerialNumberAsync(serialNumber);
        }

        /// <summary>
        /// 获取需要维护的设备（下次维护日期小于等于当前日期）
        /// </summary>
        public async Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync()
        {
            return await _equipmentRepository.GetMaintenanceRequiredEquipmentsAsync();
        }

        /// <summary>
        /// 获取特定日期范围内需要维护的设备
        /// </summary>
        public async Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync(DateTime startDate, DateTime endDate)
        {
            return await _equipmentRepository.GetMaintenanceRequiredEquipmentsAsync(startDate, endDate);
        }

        /// <summary>
        /// 记录设备维护
        /// </summary>
        public async Task<Equipment> RecordMaintenanceAsync(int equipmentId)
        {
            var equipment = await GetByIdAsync(equipmentId);
            if (equipment == null)
            {
                throw new ArgumentException($"设备ID {equipmentId} 不存在");
            }
            
            // 更新维护记录
            equipment.LastMaintenanceDate = DateTime.Now;
            
            // 计算下次维护日期
            if (equipment.MaintenanceCycle.HasValue && equipment.MaintenanceCycle > 0)
            {
                equipment.NextMaintenanceDate = DateTime.Now.AddDays(equipment.MaintenanceCycle.Value);
            }
            
            return await UpdateAsync(equipment);
        }

        /// <summary>
        /// 检查序列号是否存在
        /// </summary>
        public async Task<bool> IsSerialNumberExistsAsync(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return false;
            }
            
            var equipment = await GetBySerialNumberAsync(serialNumber);
            return equipment != null;
        }

        /// <summary>
        /// 更新设备保养周期
        /// </summary>
        public async Task<Equipment> UpdateMaintenanceCycleAsync(int equipmentId, int maintenanceCycle)
        {
            var equipment = await GetByIdAsync(equipmentId);
            if (equipment == null)
            {
                throw new ArgumentException($"设备ID {equipmentId} 不存在");
            }
            
            equipment.MaintenanceCycle = maintenanceCycle;
            
            // 如果有上次维护日期，更新下次维护日期
            if (equipment.LastMaintenanceDate.HasValue)
            {
                equipment.NextMaintenanceDate = equipment.LastMaintenanceDate.Value.AddDays(maintenanceCycle);
            }
            
            return await UpdateAsync(equipment);
        }
    }
}