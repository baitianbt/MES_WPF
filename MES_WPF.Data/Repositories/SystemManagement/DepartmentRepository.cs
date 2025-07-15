using MES_WPF.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(MesDbContext context) : base(context)
        {
        }

        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns>部门对象</returns>
        public async Task<Department> GetByCodeAsync(string deptCode)
        {
            return await _dbSet.FirstOrDefaultAsync(d => d.DeptCode == deptCode);
        }
        
        /// <summary>
        /// 获取所有部门，并组织为树形结构
        /// </summary>
        /// <returns>部门树形结构</returns>
        public async Task<IEnumerable<Department>> GetDepartmentTreeAsync()
        {
            // 获取所有部门
            var departments = await _dbSet
                .Where(d => d.Status == 1) // 1表示正常状态
                .OrderBy(d => d.SortOrder)
                .ToListAsync();

            // 找出顶级部门
            var rootDepartments = departments.Where(d => d.ParentId == null).ToList();
            
            // 注意：这里不是真正的树形结构，只是按照层级返回，前端可以用id和parentId组织树
            return rootDepartments;
        }
        
        /// <summary>
        /// 获取指定父部门下的所有子部门
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <returns>子部门列表</returns>
        public async Task<IEnumerable<Department>> GetChildrenAsync(int parentId)
        {
            return await _dbSet
                .Where(d => d.ParentId == parentId && d.Status == 1)
                .OrderBy(d => d.SortOrder)
                .ToListAsync();
        }
        
        /// <summary>
        /// 获取部门下的所有员工
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        public async Task<IEnumerable<Employee>> GetDepartmentEmployeesAsync(int deptId)
        {
            return await _context.Employees
                .Where(e => e.DeptId == deptId && e.Status == 1) // 1表示在职状态
                .OrderBy(e => e.EmployeeCode)
                .ToListAsync();
        }
        
        /// <summary>
        /// 更新部门路径
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>是否更新成功</returns>
        public async Task<bool> UpdateDeptPathAsync(int deptId)
        {
            // 获取当前部门
            var department = await _dbSet.FindAsync(deptId);
            if (department == null)
            {
                return false;
            }

            // 构建部门路径 (格式: 1,2,3)
            string deptPath = deptId.ToString();
            
            if (department.ParentId.HasValue)
            {
                // 递归获取父部门
                var parentDept = await _dbSet.FindAsync(department.ParentId.Value);
                if (parentDept != null && !string.IsNullOrEmpty(parentDept.DeptPath))
                {
                    deptPath = parentDept.DeptPath + "," + deptId;
                }
            }

            // 更新部门路径
            department.DeptPath = deptPath;
            await _context.SaveChangesAsync();
            
            // 递归更新子部门的路径
            var children = await GetChildrenAsync(deptId);
            foreach (var child in children)
            {
                await UpdateDeptPathAsync(child.Id);
            }
            
            return true;
        }
    }
} 