using MES_WPF.Core.Models;
using MES_WPF.Data.Repositories.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MES_WPF.Core.Services.SystemManagement
{
    /// <summary>
    /// 部门服务实现
    /// </summary>
    public class DepartmentService : Service<Department>, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="departmentRepository">部门仓储</param>
        /// <param name="employeeRepository">员工仓储</param>
        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IEmployeeRepository employeeRepository) 
            : base(departmentRepository)
        {
            _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        /// <summary>
        /// 根据部门编码获取部门
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns>部门</returns>
        public async Task<Department> GetByDeptCodeAsync(string deptCode)
        {
            return await _departmentRepository.GetByCodeAsync(deptCode);
        }
        
        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <returns>部门树</returns>
        public async Task<IEnumerable<Department>> GetDepartmentTreeAsync()
        {
            // 获取所有部门
            var allDepartments = await GetAllAsync();
            
            // 构建部门树
            return BuildDepartmentTree(allDepartments.ToList(), null);
        }
        
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <returns>子部门列表</returns>
        public async Task<IEnumerable<Department>> GetChildDepartmentsAsync(int parentId)
        {
            return await _departmentRepository.GetChildrenAsync(parentId);
        }
        
        /// <summary>
        /// 获取部门员工
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>员工列表</returns>
        public async Task<IEnumerable<Employee>> GetDepartmentEmployeesAsync(int deptId)
        {
            return await _employeeRepository.GetByDepartmentAsync(deptId);
        }
        
        /// <summary>
        /// 更新部门路径
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDepartmentPathAsync(int deptId)
        {
            try
            {
                // 获取部门信息
                var department = await GetByIdAsync(deptId);
                if (department == null)
                {
                    return false;
                }
                
                // 构建部门路径
                string path = await BuildDepartmentPathAsync(department);
                
                // 更新部门路径
                department.DeptPath = path;
                await UpdateAsync(department);
                
                // 更新子部门路径
                var childDepartments = await GetChildDepartmentsAsync(deptId);
                foreach (var child in childDepartments)
                {
                    await UpdateDepartmentPathAsync(child.Id);
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取部门及其所有子部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>部门列表</returns>
        public async Task<IEnumerable<Department>> GetDepartmentAndChildrenAsync(int deptId)
        {
            var result = new List<Department>();
            
            // 获取当前部门
            var department = await GetByIdAsync(deptId);
            if (department != null)
            {
                result.Add(department);
                
                // 获取所有子部门
                await GetAllChildDepartmentsAsync(deptId, result);
            }
            
            return result;
        }
        
        /// <summary>
        /// 递归获取所有子部门
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <param name="departments">部门列表</param>
        private async Task GetAllChildDepartmentsAsync(int parentId, List<Department> departments)
        {
            var children = await GetChildDepartmentsAsync(parentId);
            foreach (var child in children)
            {
                departments.Add(child);
                await GetAllChildDepartmentsAsync(child.Id, departments);
            }
        }
        
        /// <summary>
        /// 构建部门路径
        /// </summary>
        /// <param name="department">部门</param>
        /// <returns>部门路径</returns>
        private async Task<string> BuildDepartmentPathAsync(Department department)
        {
            if (department.ParentId == null)
            {
                return department.Id.ToString();
            }
            
            var parent = await GetByIdAsync(department.ParentId.Value);
            if (parent == null)
            {
                return department.Id.ToString();
            }
            
            string parentPath = await BuildDepartmentPathAsync(parent);
            return $"{parentPath},{department.Id}";
        }
        
        /// <summary>
        /// 构建部门树
        /// </summary>
        /// <param name="departments">部门列表</param>
        /// <param name="parentId">父部门ID</param>
        /// <returns>部门树</returns>
        private IEnumerable<Department> BuildDepartmentTree(List<Department> departments, int? parentId)
        {
            // 获取当前层级的部门
            var nodes = departments.Where(d => d.ParentId == parentId).ToList();
            
            // 递归构建子节点
            foreach (var node in nodes)
            {
                var children = BuildDepartmentTree(departments, node.Id);
                // 这里我们不能直接设置子节点，因为Department实体没有Children属性
                // 在实际应用中，可能需要创建一个DepartmentTreeNode类来表示树节点
                // 或者在前端构建树结构
            }
            
            return nodes;
        }
    }
} 