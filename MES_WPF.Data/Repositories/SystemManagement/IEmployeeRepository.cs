using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// 根据员工编码获取员工
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <returns>员工对象</returns>
        Task<Employee> GetByCodeAsync(string employeeCode);
        
        /// <summary>
        /// 获取指定部门下的员工列表
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int deptId);
        
        /// <summary>
        /// 根据条件筛选员工
        /// </summary>
        /// <param name="keyword">关键字(姓名、编码)</param>
        /// <param name="deptId">部门ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>包含员工列表和总数的元组</returns>
        Task<(IEnumerable<Employee> employees, int totalCount)> SearchAsync(
            string keyword = null, 
            int? deptId = null, 
            byte? status = null, 
            int pageIndex = 1, 
            int pageSize = 20);
        
        /// <summary>
        /// 检查员工编码是否已存在
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <param name="excludeId">排除的员工ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsEmployeeCodeExistsAsync(string employeeCode, int? excludeId = null);
    }
} 