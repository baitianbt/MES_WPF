using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 员工服务实现
    /// </summary>
    public class EmployeeService : Service<Employee>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="employeeRepository">员工仓储</param>
        /// <param name="userRepository">用户仓储</param>
        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository) 
            : base(employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        /// 根据员工编码获取员工
        /// </summary>
        /// <param name="employeeCode">员工编码</param>
        /// <returns>员工</returns>
        public async Task<Employee> GetByEmployeeCodeAsync(string employeeCode)
        {
            return await _employeeRepository.GetByCodeAsync(employeeCode);
        }
        
        /// <summary>
        /// 根据部门ID获取员工列表
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        public async Task<IEnumerable<Employee>> GetByDepartmentIdAsync(int deptId)
        {
            return await _employeeRepository.GetByDepartmentAsync(deptId);
        }
        
        /// <summary>
        /// 获取在职员工
        /// </summary>
        /// <returns>在职员工列表</returns>
        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            //return await _employeeRepository.GetByStatusAsync(1); // 1表示在职
            throw new NotImplementedException("Method not implemented yet.");
        }
        
        /// <summary>
        /// 获取离职员工
        /// </summary>
        /// <returns>离职员工列表</returns>
        public async Task<IEnumerable<Employee>> GetLeaveEmployeesAsync()
        {
            //return await _employeeRepository.GetByStatusAsync(2); // 2表示离职
            throw new NotImplementedException("Method not implemented yet.");

        }

        /// <summary>
        /// 员工离职
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <param name="leaveDate">离职日期</param>
        /// <returns>是否成功</returns>
        public async Task<bool> LeaveAsync(int employeeId, DateTime leaveDate)
        {
            try
            {
                var employee = await GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return false;
                }
                
                employee.LeaveDate = leaveDate;
                employee.Status = 2; // 2表示离职
                employee.UpdateTime = DateTime.Now;
                
                await UpdateAsync(employee);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 员工调岗
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <param name="newDeptId">新部门ID</param>
        /// <param name="newPosition">新职位</param>
        /// <returns>是否成功</returns>
        public async Task<bool> TransferAsync(int employeeId, int newDeptId, string newPosition)
        {
            try
            {
                var employee = await GetByIdAsync(employeeId);
                if (employee == null)
                {
                    return false;
                }
                
                employee.DeptId = newDeptId;
                employee.Position = newPosition;
                employee.UpdateTime = DateTime.Now;
                
                await UpdateAsync(employee);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取员工关联的用户
        /// </summary>
        /// <param name="employeeId">员工ID</param>
        /// <returns>用户</returns>
        public async Task<User> GetRelatedUserAsync(int employeeId)
        {
            return await _userRepository.GetByIdAsync(employeeId);
        }
    }
} 