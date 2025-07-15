using MES_WPF.Core.Models;
using MES_WPF.Data;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace MES_WPF.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(MesDbContext context)
        {
            try
            {
                Debug.WriteLine("开始初始化数据库...");
                
                // 确保数据库已创建
                await context.Database.EnsureCreatedAsync();
                Debug.WriteLine("数据库创建完成");
                
                // 如果已经有用户数据，则不再初始化
                if (await context.Users.AnyAsync())
                {
                    Debug.WriteLine("数据库已有数据，跳过初始化");
                    return;
                }
                
                Debug.WriteLine("开始添加部门数据");
                // 添加部门数据
                var adminDept = new Department
                {
                    DeptName = "系统管理部",
                    DeptCode = "ADMIN",
                    ParentId = null,
                    DeptPath = "/ADMIN",
                    Leader = "系统管理员",
                    Phone = "010-12345678",
                    Email = "admin@example.com",
                    SortOrder = 1,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var productionDept = new Department
                {
                    DeptName = "生产部",
                    DeptCode = "PROD",
                    ParentId = null,
                    DeptPath = "/PROD",
                    Leader = "生产主管",
                    Phone = "010-12345679",
                    Email = "prod@example.com",
                    SortOrder = 2,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var qualityDept = new Department
                {
                    DeptName = "质量管理部",
                    DeptCode = "QA",
                    ParentId = null,
                    DeptPath = "/QA",
                    Leader = "质量主管",
                    Phone = "010-12345680",
                    Email = "qa@example.com",
                    SortOrder = 3,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                context.Departments.Add(adminDept);
                context.Departments.Add(productionDept);
                context.Departments.Add(qualityDept);
                await context.SaveChangesAsync(); // 先保存部门，获取自增ID
                Debug.WriteLine($"部门数据添加完成，ID: {adminDept.Id}, {productionDept.Id}, {qualityDept.Id}");
                
                Debug.WriteLine("开始添加员工数据");
                // 添加员工数据
                var adminEmployee = new Employee
                {
                    EmployeeName = "系统管理员",
                    EmployeeCode = "EMP001",
                    DeptId = adminDept.Id,
                    Gender = 1, // 1:男
                    Phone = "13800138000",
                    Email = "admin@example.com",
                    Position = "系统管理员",
                    EntryDate = DateTime.Now.AddYears(-2),
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var prodEmployee = new Employee
                {
                    EmployeeName = "生产管理员",
                    EmployeeCode = "EMP002",
                    DeptId = productionDept.Id,
                    Gender = 1, // 1:男
                    Phone = "13800138001",
                    Email = "prod@example.com",
                    Position = "生产主管",
                    EntryDate = DateTime.Now.AddYears(-1),
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var qaEmployee = new Employee
                {
                    EmployeeName = "质检员",
                    EmployeeCode = "EMP003",
                    DeptId = qualityDept.Id,
                    Gender = 2, // 2:女
                    Phone = "13800138002",
                    Email = "qa@example.com",
                    Position = "质检主管",
                    EntryDate = DateTime.Now.AddMonths(-6),
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                context.Employees.Add(adminEmployee);
                context.Employees.Add(prodEmployee);
                context.Employees.Add(qaEmployee);
                await context.SaveChangesAsync(); // 先保存员工，获取自增ID
                Debug.WriteLine($"员工数据添加完成，ID: {adminEmployee.Id}, {prodEmployee.Id}, {qaEmployee.Id}");
                
                Debug.WriteLine("开始添加角色数据");
                // 添加角色数据
                var adminRole = new Role
                {
                    RoleName = "超级管理员",
                    RoleCode = "ADMIN",
                    RoleType = 1, // 系统角色
                    Status = 1,
                    SortOrder = 1,
                    CreateBy = 0, // 系统创建
                    CreateTime = DateTime.Now,
                    Remark = "系统超级管理员，拥有所有权限"
                };
                
                var prodRole = new Role
                {
                    RoleName = "生产管理员",
                    RoleCode = "PROD_MANAGER",
                    RoleType = 2, // 业务角色
                    Status = 1,
                    SortOrder = 2,
                    CreateBy = 0, // 系统创建
                    CreateTime = DateTime.Now,
                    Remark = "生产管理员，负责生产计划和执行"
                };
                
                var qaRole = new Role
                {
                    RoleName = "质检员",
                    RoleCode = "QA",
                    RoleType = 2, // 业务角色
                    Status = 1,
                    SortOrder = 3,
                    CreateBy = 0, // 系统创建
                    CreateTime = DateTime.Now,
                    Remark = "质检员，负责质量检验"
                };
                
                context.Roles.Add(adminRole);
                context.Roles.Add(prodRole);
                context.Roles.Add(qaRole);
                await context.SaveChangesAsync(); // 先保存角色，获取自增ID
                Debug.WriteLine($"角色数据添加完成，ID: {adminRole.Id}, {prodRole.Id}, {qaRole.Id}");
                
                Debug.WriteLine("开始添加权限数据");
                // 添加权限数据
                var systemMgmtPerm = new Permission
                {
                    PermissionName = "系统管理",
                    PermissionCode = "SYSTEM",
                    PermissionType = 1, // 菜单
                    ParentId = null,
                    Path = "/system",
                    Component = "Layout",
                    Icon = "cog",
                    SortOrder = 1,
                    IsVisible = true,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                context.Permissions.Add(systemMgmtPerm);
                await context.SaveChangesAsync(); // 先保存父权限，获取自增ID
                Debug.WriteLine($"父权限数据添加完成，ID: {systemMgmtPerm.Id}");
                
                var userMgmtPerm = new Permission
                {
                    PermissionName = "用户管理",
                    PermissionCode = "USER",
                    PermissionType = 1, // 菜单
                    ParentId = systemMgmtPerm.Id,
                    Path = "/system/user",
                    Component = "system/user/index",
                    Icon = "user",
                    SortOrder = 1,
                    IsVisible = true,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var roleMgmtPerm = new Permission
                {
                    PermissionName = "角色管理",
                    PermissionCode = "ROLE",
                    PermissionType = 1, // 菜单
                    ParentId = systemMgmtPerm.Id,
                    Path = "/system/role",
                    Component = "system/role/index",
                    Icon = "peoples",
                    SortOrder = 2,
                    IsVisible = true,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                context.Permissions.Add(userMgmtPerm);
                context.Permissions.Add(roleMgmtPerm);
                await context.SaveChangesAsync(); // 保存子权限，获取自增ID
                Debug.WriteLine($"子权限数据添加完成，ID: {userMgmtPerm.Id}, {roleMgmtPerm.Id}");
                
                Debug.WriteLine("开始添加角色权限关联数据");
                // 给角色分配权限
                var adminRoleSystemPerm = new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = systemMgmtPerm.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                var adminRoleUserPerm = new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = userMgmtPerm.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                var adminRoleRolePerm = new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = roleMgmtPerm.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                var prodRoleSystemPerm = new RolePermission
                {
                    RoleId = prodRole.Id,
                    PermissionId = systemMgmtPerm.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                context.RolePermissions.Add(adminRoleSystemPerm);
                context.RolePermissions.Add(adminRoleUserPerm);
                context.RolePermissions.Add(adminRoleRolePerm);
                context.RolePermissions.Add(prodRoleSystemPerm);
                await context.SaveChangesAsync();
                Debug.WriteLine("角色权限关联数据添加完成");
                
                Debug.WriteLine("开始添加用户数据");
                // 添加用户数据
                var adminUser = new User
                {
                    Username = "admin",
                    Password = EncryptPassword("123456"), // 加密密码
                    RealName = "系统管理员",
                    Email = "admin@example.com",
                    Mobile = "13800138000",
                    EmployeeId = adminEmployee.Id,
                    LastLoginIp="127.0.0.1",
                    Status = 1,
                    CreateTime = DateTime.Now,
                    PasswordUpdateTime = DateTime.Now
                };
                
                var prodUser = new User
                {
                    Username = "production",
                    Password = EncryptPassword("123456"), // 加密密码
                    RealName = "生产管理员",
                    Email = "prod@example.com",
                    Mobile = "13800138001",
                    EmployeeId = prodEmployee.Id,
                    LastLoginIp = "127.0.0.1",

                    Status = 1,
                    CreateTime = DateTime.Now,
                    PasswordUpdateTime = DateTime.Now
                };
                
                var qaUser = new User
                {
                    Username = "quality",
                    Password = EncryptPassword("123456"), // 加密密码
                    RealName = "质检员",
                    Email = "qa@example.com",
                    Mobile = "13800138002",
                    EmployeeId = qaEmployee.Id,
                    LastLoginIp = "127.0.0.1",

                    Status = 1,
                    CreateTime = DateTime.Now,
                    PasswordUpdateTime = DateTime.Now
                };
                
                context.Users.Add(adminUser);
                context.Users.Add(prodUser);
                context.Users.Add(qaUser);
                await context.SaveChangesAsync(); // 保存用户，获取自增ID
                Debug.WriteLine($"用户数据添加完成，ID: {adminUser.Id}, {prodUser.Id}, {qaUser.Id}");
                
                Debug.WriteLine("开始添加用户角色关联数据");
                // 用户角色关联
                var adminUserRole = new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                var prodUserRole = new UserRole
                {
                    UserId = prodUser.Id,
                    RoleId = prodRole.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                var qaUserRole = new UserRole
                {
                    UserId = qaUser.Id,
                    RoleId = qaRole.Id,
                    CreateBy = 0,
                    CreateTime = DateTime.Now
                };
                
                context.UserRoles.Add(adminUserRole);
                context.UserRoles.Add(prodUserRole);
                context.UserRoles.Add(qaUserRole);
                await context.SaveChangesAsync();
                Debug.WriteLine("用户角色关联数据添加完成");
                
                Debug.WriteLine("开始添加系统配置数据");
                // 添加系统配置
                var systemConfig1 = new SystemConfig
                {
                    ConfigKey = "SYSTEM_NAME",
                    ConfigValue = "MES生产管理系统",
                    ConfigName = "系统名称",
                    ConfigType = "system",
                    IsSystem = true,
                    Status = 1,
                    CreateBy = 0,
                    CreateTime = DateTime.Now,
                    Remark = "系统名称配置"
                };
                
                var systemConfig2 = new SystemConfig
                {
                    ConfigKey = "SYSTEM_LOGO",
                    ConfigValue = "/logo.png",
                    ConfigName = "系统Logo",
                    ConfigType = "system",
                    IsSystem = true,
                    Status = 1,
                    CreateBy = 0,
                    CreateTime = DateTime.Now,
                    Remark = "系统Logo配置"
                };
                
                context.SystemConfigs.Add(systemConfig1);
                context.SystemConfigs.Add(systemConfig2);
                await context.SaveChangesAsync();
                Debug.WriteLine("系统配置数据添加完成");
                
                Debug.WriteLine("开始添加数据字典数据");
                // 添加数据字典
                var statusDict = new Dictionary
                {
                    DictType = "status",
                    DictName = "状态",
                    Status = 1,
                    CreateBy = 0,
                    CreateTime = DateTime.Now,
                    Remark = "通用状态"
                };
                
                context.Dictionaries.Add(statusDict);
                await context.SaveChangesAsync(); // 保存字典，获取自增ID
                Debug.WriteLine($"数据字典数据添加完成，ID: {statusDict.Id}");
                
                Debug.WriteLine("开始添加字典项数据");
                // 添加字典项
                var statusItem1 = new DictionaryItem
                {
                    DictId = statusDict.Id,
                    ItemValue = "1",
                    ItemText = "启用",
                    ItemDesc = "启用状态",
                    SortOrder = 1,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                var statusItem2 = new DictionaryItem
                {
                    DictId = statusDict.Id,
                    ItemValue = "2",
                    ItemText = "禁用",
                    ItemDesc = "禁用状态",
                    SortOrder = 2,
                    Status = 1,
                    CreateTime = DateTime.Now
                };
                
                context.DictionaryItems.Add(statusItem1);
                context.DictionaryItems.Add(statusItem2);
                
                await context.SaveChangesAsync();
                Debug.WriteLine("字典项数据添加完成");
                
                Debug.WriteLine("数据库初始化完成!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"数据库初始化失败: {ex.Message}");
                Debug.WriteLine($"异常堆栈: {ex.StackTrace}");
                // 重新抛出异常，让调用者处理
                throw;
            }
        }
        
        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="password">原始密码</param>
        /// <returns>加密后的密码</returns>
        private static string EncryptPassword(string password)
        {
            // 使用SHA256加密
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
} 