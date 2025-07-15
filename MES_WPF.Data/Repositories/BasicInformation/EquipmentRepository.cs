using MES_WPF.Model.BasicInformation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.BasicInformation
{
    public class EquipmentRepository : Repository<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据资源ID获取设备
        /// </summary>
        public async Task<Equipment> GetByResourceIdAsync(int resourceId)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.ResourceId == resourceId);
        }

        /// <summary>
        /// 根据序列号获取设备
        /// </summary>
        public async Task<Equipment> GetBySerialNumberAsync(string serialNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.SerialNumber == serialNumber);
        }

        /// <summary>
        /// 获取需要维护的设备（下次维护日期小于等于当前日期）
        /// </summary>
        public async Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync()
        {
            var today = DateTime.Today;
            return await _dbSet.Where(e => e.NextMaintenanceDate.HasValue && e.NextMaintenanceDate.Value <= today)
                              .Include(e => e.Resource)
                              .ToListAsync();
        }

        /// <summary>
        /// 获取特定日期范围内需要维护的设备
        /// </summary>
        public async Task<IEnumerable<Equipment>> GetMaintenanceRequiredEquipmentsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(e => e.NextMaintenanceDate.HasValue && 
                                        e.NextMaintenanceDate.Value >= startDate && 
                                        e.NextMaintenanceDate.Value <= endDate)
                              .Include(e => e.Resource)
                              .ToListAsync();
        }
    }
} 