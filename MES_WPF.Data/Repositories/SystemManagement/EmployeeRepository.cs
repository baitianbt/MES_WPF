using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据员工编码获取员工
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <returns>员工对象</returns>
        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }
        
        /// <summary>
        /// 获取指定部门下的员工列表
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int deptId)
        {
            return await _dbSet
                .Where(e => e.DeptId == deptId && e.Status == 1) // 1表示在职状态
                .OrderBy(e => e.EmployeeCode)
                .ToListAsync();
        }
        
        /// <summary>
        /// 根据条件筛选员工
        /// </summary>
        /// <param name="keyword">关键字(姓名、编码)</param>
        /// <param name="deptId">部门ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>包含员工列表和总数的元组</returns>
        public async Task<(IEnumerable<Employee> employees, int totalCount)> SearchAsync(
            string keyword = null, 
            int? deptId = null, 
            byte? status = null, 
            int pageIndex = 1, 
            int pageSize = 20)
        {
            var query = _dbSet.AsQueryable();
            
            // 应用筛选条件
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(e => e.EmployeeName.Contains(keyword) || e.EmployeeCode.Contains(keyword));
            }
            
            if (deptId.HasValue)
            {
                query = query.Where(e => e.DeptId == deptId.Value);
            }
            
            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }
            
            // 获取总数
            int totalCount = await query.CountAsync();
            
            // 分页并返回结果
            var employees = await query
                .OrderBy(e => e.EmployeeCode)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return (employees, totalCount);
        }
        
        /// <summary>
        /// 检查员工编码是否已存在
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <param name="excludeId">排除的员工ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsEmployeeCodeExistsAsync(string employeeCode, int? excludeId = null)
        {
            var query = _dbSet.Where(e => e.EmployeeCode == employeeCode);
            
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
} 