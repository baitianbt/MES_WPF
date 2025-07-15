using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 部门服务接口
    /// </summary>
    public interface IDepartmentService : IService<Department>
    {
        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns>部门</returns>
        Task<Department> GetByDeptCodeAsync(string deptCode);
        
        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <returns>部门树</returns>
        Task<IEnumerable<Department>> GetDepartmentTreeAsync();
        
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <returns>子部门列表</returns>
        Task<IEnumerable<Department>> GetChildDepartmentsAsync(int parentId);
        
        /// <summary>
        /// 获取部门员工
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        Task<IEnumerable<Employee>> GetDepartmentEmployeesAsync(int deptId);
        
        /// <summary>
        /// 更新部门路径
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateDepartmentPathAsync(int deptId);
        
        /// <summary>
        /// 获取部门及其所有子部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>部门列表</returns>
        Task<IEnumerable<Department>> GetDepartmentAndChildrenAsync(int deptId);
    }
} 