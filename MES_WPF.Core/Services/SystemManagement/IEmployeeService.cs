using MES_WPF.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 员工服务接口
    /// </summary>
    public interface IEmployeeService : IService<Employee>
    {
        /// <summary>
        /// 根据员工编码获取员工
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <returns>员工</returns>
        Task<Employee> GetByEmployeeCodeAsync(string employeeCode);
        
        /// <summary>
        /// 根据部门ID获取员工列表
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int deptId);
        
        /// <summary>
        /// 获取在职员工
        /// </summary>
        /// <returns>在职员工列表</returns>
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
        
        /// <summary>
        /// 获取离职员工
        /// </summary>
        /// <returns>离职员工列表</returns>
        Task<IEnumerable<Employee>> GetLeaveEmployeesAsync();
        
        /// <summary>
        /// 员工离职
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <param name="leaveDate">离职日期</param>
        /// <returns>是否成功</returns>
        Task<bool> LeaveAsync(int employeeId, DateTime leaveDate);
        
        /// <summary>
        /// 员工调岗
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <param name="newDeptId">新部门ID</param>
        /// <param name="newPosition">新职位</param>
        /// <returns>是否成功</returns>
        Task<bool> TransferAsync(int employeeId, int newDeptId, string newPosition);
        
        /// <summary>
        /// 获取员工关联的用户
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <returns>用户</returns>
        Task<User> GetRelatedUserAsync(int employeeId);
    }
} 