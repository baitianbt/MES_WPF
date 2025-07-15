using MES_WPF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns>部门对象</returns>
        Task<Department> GetByCodeAsync(string deptCode);
        
        /// <summary>
        /// 获取所有部门，并组织为树形结构
        /// </summary>
        /// <returns>部门树形结构</returns>
        Task<IEnumerable<Department>> GetDepartmentTreeAsync();
        
        /// <summary>
        /// 获取指定父部门下的所有子部门
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <returns>子部门列表</returns>
        Task<IEnumerable<Department>> GetChildrenAsync(int parentId);
        
        /// <summary>
        /// 获取部门下的所有员工
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        Task<IEnumerable<Employee>> GetDepartmentEmployeesAsync(int deptId);
        
        /// <summary>
        /// 更新部门路径
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateDeptPathAsync(int deptId);
    }
} 