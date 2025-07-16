using MES_WPF.Core.Models;
using MES_WPF.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Model.EquipmentManagement;

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
                
                // 基础信息模块数据初始化
                await InitializeBasicInfo(context);
                
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
        /// 初始化基础信息模块数据
        /// </summary>
        private static async Task InitializeBasicInfo(MesDbContext context)
        {
            Debug.WriteLine("开始初始化基础信息模块数据...");

            try
            {
                // 初始化产品数据
                await InitializeProductsAsync(context);
                
                // 初始化资源数据
                await InitializeResourcesAsync(context);
                
                // 初始化工序数据
                await InitializeOperationsAsync(context);
                
                // 初始化工艺路线数据
                await InitializeProcessRoutesAsync(context);
                
                // 初始化BOM数据
                await InitializeBOMsAsync(context);
                
                // 初始化设备管理模块数据
                
                Debug.WriteLine("基础信息模块数据初始化完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"基础信息模块数据初始化失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 初始化产品数据
        /// </summary>
        private static async Task InitializeProductsAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加产品数据");
            
            var product1 = new Product
            {
                ProductCode = "P001",
                ProductName = "电子控制器",
                ProductType = 1, // 成品
                Specification = "ECU-V1.0",
                Unit = "个",
                Description = "车载电子控制单元",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product2 = new Product
            {
                ProductCode = "P002",
                ProductName = "PCB主板",
                ProductType = 2, // 半成品
                Specification = "PCB-150x100",
                Unit = "块",
                Description = "电子控制器用PCB板",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product3 = new Product
            {
                ProductCode = "M001",
                ProductName = "芯片",
                ProductType = 3, // 原材料
                Specification = "MCU-STM32",
                Unit = "个",
                Description = "控制芯片",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product4 = new Product
            {
                ProductCode = "M002",
                ProductName = "电阻",
                ProductType = 3, // 原材料
                Specification = "10kΩ",
                Unit = "个",
                Description = "标准电阻",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product5 = new Product
            {
                ProductCode = "M003",
                ProductName = "电容",
                ProductType = 3, // 原材料
                Specification = "100nF",
                Unit = "个",
                Description = "标准电容",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            context.Add(product1);
            context.Add(product2);
            context.Add(product3);
            context.Add(product4);
            context.Add(product5);
            await context.SaveChangesAsync();
            Debug.WriteLine($"产品数据添加完成，ID: {product1.Id}, {product2.Id}, {product3.Id}, {product4.Id}, {product5.Id}");
        }
        
        /// <summary>
        /// 初始化资源数据
        /// </summary>
        private static async Task InitializeResourcesAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加资源数据");
            
            // 获取生产部门ID
            var productionDept = await context.Departments.FirstOrDefaultAsync(d => d.DeptCode == "PROD");
            int deptId = productionDept?.Id ?? 1;

            var resource1 = new Resource
            {
                ResourceCode = "R001",
                ResourceName = "SMT贴片机",
                ResourceType = 1, // 设备
                DepartmentId = deptId,
                Status = 1, // 可用
                Description = "全自动SMT贴片设备",
                CreateTime = DateTime.Now
            };

            var resource2 = new Resource
            {
                ResourceCode = "R002",
                ResourceName = "焊接工位",
                ResourceType = 1, // 设备
                DepartmentId = deptId,
                Status = 1, // 可用
                Description = "手动焊接工位",
                CreateTime = DateTime.Now
            };

            var resource3 = new Resource
            {
                ResourceCode = "R003",
                ResourceName = "测试台",
                ResourceType = 1, // 设备
                DepartmentId = deptId,
                Status = 1, // 可用
                Description = "电子产品测试设备",
                CreateTime = DateTime.Now
            };

            context.Add(resource1);
            context.Add(resource2);
            context.Add(resource3);
            await context.SaveChangesAsync();
            Debug.WriteLine($"资源数据添加完成，ID: {resource1.Id}, {resource2.Id}, {resource3.Id}");

            // 添加设备详细信息
            var equipment1 = new Equipment
            {
                ResourceId = resource1.Id,
                EquipmentModel = "SMT-3000",
                Manufacturer = "电子设备制造厂",
                SerialNumber = "SMT30001001",
                PurchaseDate = DateTime.Now.AddYears(-2),
                WarrantyPeriod = 36, // 36个月
                MaintenanceCycle = 30, // 30天
                LastMaintenanceDate = DateTime.Now.AddDays(-15),
                NextMaintenanceDate = DateTime.Now.AddDays(15),
                IpAddress = "192.168.1.101",
                OpcUaEndpoint = "opc.tcp://"

            };

            var equipment2 = new Equipment
            {
                ResourceId = resource2.Id,
                EquipmentModel = "WELD-200",
                Manufacturer = "焊接设备有限公司",
                SerialNumber = "WELD2001002",
                PurchaseDate = DateTime.Now.AddYears(-1),
                WarrantyPeriod = 24, // 24个月
                MaintenanceCycle = 60, // 60天
                LastMaintenanceDate = DateTime.Now.AddDays(-30),
                NextMaintenanceDate = DateTime.Now.AddDays(30),
                IpAddress = "192.168.1.102"
            };

            var equipment3 = new Equipment
            {
                ResourceId = resource3.Id,
                EquipmentModel = "TEST-500",
                Manufacturer = "测试设备制造商",
                SerialNumber = "TEST5001003",
                PurchaseDate = DateTime.Now.AddMonths(-6),
                WarrantyPeriod = 12, // 12个月
                MaintenanceCycle = 45, // 45天
                LastMaintenanceDate = DateTime.Now.AddDays(-20),
                NextMaintenanceDate = DateTime.Now.AddDays(25),
                IpAddress = "192.168.1.103"
            };

            context.Add(equipment1);
            context.Add(equipment2);
            context.Add(equipment3);
            await context.SaveChangesAsync();
            Debug.WriteLine($"设备数据添加完成，ID: {equipment1.ResourceId}, {equipment2.ResourceId}, {equipment3.ResourceId}");
        }

        /// <summary>
        /// 初始化工序数据
        /// </summary>
        private static async Task InitializeOperationsAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加工序数据");

            var operation1 = new Operation
            {
                OperationCode = "OP001",
                OperationName = "SMT贴片",
                OperationType = 1, // 加工
                Department = "生产部",
                Description = "电子元件SMT贴片工序",
                StandardTime = 10.0m, // 10分钟
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var operation2 = new Operation
            {
                OperationCode = "OP002",
                OperationName = "手工焊接",
                OperationType = 1, // 加工
                Department = "生产部",
                Description = "电子元件手工焊接工序",
                StandardTime = 15.0m, // 15分钟
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var operation3 = new Operation
            {
                OperationCode = "OP003",
                OperationName = "功能测试",
                OperationType = 2, // 检验
                Department = "质量管理部",
                Description = "产品功能测试工序",
                StandardTime = 8.0m, // 8分钟
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var operation4 = new Operation
            {
                OperationCode = "OP004",
                OperationName = "包装",
                OperationType = 1, // 加工
                Department = "生产部",
                Description = "产品包装工序",
                StandardTime = 5.0m, // 5分钟
                IsActive = true,
                CreateTime = DateTime.Now
            };

            context.Add(operation1);
            context.Add(operation2);
            context.Add(operation3);
            context.Add(operation4);
            await context.SaveChangesAsync();
            Debug.WriteLine($"工序数据添加完成，ID: {operation1.Id}, {operation2.Id}, {operation3.Id}, {operation4.Id}");
        }

        /// <summary>
        /// 初始化工艺路线数据
        /// </summary>
        private static async Task InitializeProcessRoutesAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加工艺路线数据");

            // 获取产品
            var product = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "P001");
            if (product == null)
            {
                Debug.WriteLine("找不到产品P001，跳过工艺路线初始化");
                return;
            }

            // 创建工艺路线
            var route = new ProcessRoute
            {
                RouteCode = "RT001",
                RouteName = "电子控制器标准工艺",
                ProductId = product.Id,
                Version = "1.0",
                Status = 3, // 已发布
                IsDefault = true,
                CreateTime = DateTime.Now
            };

            context.Add(route);
            await context.SaveChangesAsync();
            Debug.WriteLine($"工艺路线添加完成，ID: {route.Id}");

            // 获取工序
            var operations = await context.Set<Operation>().ToListAsync();
            if (operations.Count < 3)
            {
                Debug.WriteLine("工序数据不足，跳过工艺步骤初始化");
                return;
            }

            // 添加工艺步骤
            var step1 = new RouteStep
            {
                RouteId = route.Id,
                OperationId = operations[0].Id, // SMT贴片
                StepNo = 10,
                SetupTime = 15.0m,
                ProcessTime = 10.0m,
                WaitTime = 5.0m,
                Description = "PCB板SMT贴片",
                IsKeyOperation = true,
                IsQualityCheckPoint = false
            };

            var step2 = new RouteStep
            {
                RouteId = route.Id,
                OperationId = operations[1].Id, // 手工焊接
                StepNo = 20,
                SetupTime = 5.0m,
                ProcessTime = 15.0m,
                WaitTime = 0.0m,
                Description = "焊接特殊元件",
                IsKeyOperation = false,
                IsQualityCheckPoint = false
            };

            var step3 = new RouteStep
            {
                RouteId = route.Id,
                OperationId = operations[2].Id, // 功能测试
                StepNo = 30,
                SetupTime = 3.0m,
                ProcessTime = 8.0m,
                WaitTime = 0.0m,
                Description = "电气性能测试",
                IsKeyOperation = true,
                IsQualityCheckPoint = true
            };

            var step4 = new RouteStep
            {
                RouteId = route.Id,
                OperationId = operations[3].Id, // 包装
                StepNo = 40,
                SetupTime = 2.0m,
                ProcessTime = 5.0m,
                WaitTime = 0.0m,
                Description = "装箱包装",
                IsKeyOperation = false,
                IsQualityCheckPoint = false
            };

            context.Add(step1);
            context.Add(step2);
            context.Add(step3);
            context.Add(step4);
            await context.SaveChangesAsync();
            Debug.WriteLine($"工艺步骤添加完成，ID: {step1.Id}, {step2.Id}, {step3.Id}, {step4.Id}");
        }

        /// <summary>
        /// 初始化BOM数据
        /// </summary>
        private static async Task InitializeBOMsAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加BOM数据");

            // 获取产品
            var mainProduct = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "P001");
            var pcbProduct = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "P002");
            var chipProduct = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "M001");
            var resistorProduct = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "M002");
            var capacitorProduct = await context.Set<Product>().FirstOrDefaultAsync(p => p.ProductCode == "M003");

            if (mainProduct == null || pcbProduct == null)
            {
                Debug.WriteLine("找不到必要的产品数据，跳过BOM初始化");
                return;
            }

            // 创建BOM
            var bom = new BOM
            {
                BomCode = "BOM001",
                BomName = "电子控制器BOM",
                ProductId = mainProduct.Id,
                Version = "1.0",
                Status = 3, // 已发布
                EffectiveDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(5),
                IsDefault = true,
                CreateTime = DateTime.Now
            };

            context.Add(bom);
            await context.SaveChangesAsync();
            Debug.WriteLine($"BOM添加完成，ID: {bom.Id}");

            // 添加BOM项
            var bomItem1 = new BOMItem
            {
                BomId = bom.Id,
                MaterialId = pcbProduct.Id,
                Quantity = 1.0m,
                UnitId = pcbProduct.Unit,
                Position = "主板",
                IsKey = true,
                LossRate = 1.0m, // 1%损耗率
                Remark = "主PCB板"
            };

            var bomItems = new List<BOMItem> { bomItem1 };

            if (chipProduct != null)
            {
                var bomItem2 = new BOMItem
                {
                    BomId = bom.Id,
                    MaterialId = chipProduct.Id,
                    Quantity = 2.0m,
                    UnitId = chipProduct.Unit,
                    Position = "U1, U2",
                    IsKey = true,
                    LossRate = 0.5m, // 0.5%损耗率
                    Remark = "控制芯片"
                };
                bomItems.Add(bomItem2);
            }

            if (resistorProduct != null)
            {
                var bomItem3 = new BOMItem
                {
                    BomId = bom.Id,
                    MaterialId = resistorProduct.Id,
                    Quantity = 10.0m,
                    UnitId = resistorProduct.Unit,
                    Position = "R1-R10",
                    IsKey = false,
                    LossRate = 2.0m, // 2%损耗率
                    Remark = "电阻"
                };
                bomItems.Add(bomItem3);
            }

            if (capacitorProduct != null)
            {
                var bomItem4 = new BOMItem
                {
                    BomId = bom.Id,
                    MaterialId = capacitorProduct.Id,
                    Quantity = 5.0m,
                    UnitId = capacitorProduct.Unit,
                    Position = "C1-C5",
                    IsKey = false,
                    LossRate = 2.0m, // 2%损耗率
                    Remark = "电容"
                };
                bomItems.Add(bomItem4);
            }

            foreach (var item in bomItems)
            {
                context.Add(item);
            }

            await context.SaveChangesAsync();
            Debug.WriteLine($"BOM明细添加完成，共{bomItems.Count}条");
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

        /// <summary>
        /// 初始化基础信息模块数据
        /// </summary>
        private static async Task InitializeBasicInformationDataAsync(MesDbContext context)
        {
            Debug.WriteLine("开始初始化基础信息模块数据...");

            // 1. 添加产品数据
            Debug.WriteLine("开始添加产品数据");
            var product1 = new MES_WPF.Model.BasicInformation.Product
            {
                ProductCode = "P001",
                ProductName = "电子控制器",
                ProductType = 1, // 成品
                Specification = "ECU-V1.0",
                Unit = "个",
                Description = "车载电子控制单元",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product2 = new MES_WPF.Model.BasicInformation.Product
            {
                ProductCode = "P002",
                ProductName = "PCB主板",
                ProductType = 2, // 半成品
                Specification = "PCB-150x100",
                Unit = "块",
                Description = "电子控制器用PCB板",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product3 = new MES_WPF.Model.BasicInformation.Product
            {
                ProductCode = "M001",
                ProductName = "芯片",
                ProductType = 3, // 原材料
                Specification = "MCU-STM32",
                Unit = "个",
                Description = "控制芯片",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product4 = new MES_WPF.Model.BasicInformation.Product
            {
                ProductCode = "M002",
                ProductName = "电阻",
                ProductType = 3, // 原材料
                Specification = "10kΩ",
                Unit = "个",
                Description = "标准电阻",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            var product5 = new MES_WPF.Model.BasicInformation.Product
            {
                ProductCode = "M003",
                ProductName = "电容",
                ProductType = 3, // 原材料
                Specification = "100nF",
                Unit = "个",
                Description = "标准电容",
                IsActive = true,
                CreateTime = DateTime.Now
            };

            context.Add(product1);
            context.Add(product2);
            context.Add(product3);
            context.Add(product4);
            context.Add(product5);
            await context.SaveChangesAsync();
            Debug.WriteLine($"产品数据添加完成，ID: {product1.Id}, {product2.Id}, {product3.Id}, {product4.Id}, {product5.Id}");

            // 继续添加其他基础信息数据...
        }
   
   
   
                /// <summary>
        /// 初始化维护工单数据
        /// </summary>
        private static async Task InitializeMaintenanceOrdersAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加维护工单数据");
            
            // 检查是否已存在维护工单数据
            if (await context.Set<MaintenanceOrder>().AnyAsync())
            {
                Debug.WriteLine("维护工单数据已存在，跳过初始化");
                return;
            }
            
            // 获取设备数据
            var equipments = await context.Set<Equipment>().ToListAsync();
            if (equipments == null || equipments.Count == 0)
            {
                Debug.WriteLine("找不到设备数据，跳过维护工单初始化");
                return;
            }
            
            // 获取维护计划数据
            var maintenancePlans = await context.Set<EquipmentMaintenancePlan>().ToListAsync();
            if (maintenancePlans == null || maintenancePlans.Count == 0)
            {
                Debug.WriteLine("找不到维护计划数据，跳过维护工单初始化");
                return;
            }
            
            // 获取用户数据
            var users = await context.Users.ToListAsync();
            int reportBy = users.FirstOrDefault(u => u.Username == "admin")?.Id ?? 1;
            int assignedTo = users.FirstOrDefault(u => u.Username == "production")?.Id ?? 1;

            // 创建计划维护工单
            var order1 = new MaintenanceOrder
            {
                OrderCode = "MO001",
                OrderType = 1, // 计划维护
                EquipmentId = equipments[0].ResourceId,
                MaintenancePlanId = maintenancePlans[0].Id, // SMT贴片机日常保养
                Priority = 5,
                Status = 4, // 已完成
                PlanStartTime = DateTime.Now.AddDays(-1),
                PlanEndTime = DateTime.Now.AddDays(-1).AddHours(1),
                ActualStartTime = DateTime.Now.AddDays(-1).AddMinutes(10),
                ActualEndTime = DateTime.Now.AddDays(-1).AddMinutes(40),
                ReportBy = reportBy,
                AssignedTo = assignedTo,
                CreateTime = DateTime.Now.AddDays(-1),
                UpdateTime = DateTime.Now.AddDays(-1).AddMinutes(40),
                Remark = "日常保养工单"
            };

            var order2 = new MaintenanceOrder
            {
                OrderCode = "MO002",
                OrderType = 1, // 计划维护
                EquipmentId = equipments[1].ResourceId,
                MaintenancePlanId = maintenancePlans[2].Id, // 焊接工位周检
                Priority = 6,
                Status = 3, // 处理中
                PlanStartTime = DateTime.Now,
                PlanEndTime = DateTime.Now.AddHours(2),
                ActualStartTime = DateTime.Now.AddMinutes(-30),
                ActualEndTime = null,
                ReportBy = reportBy,
                AssignedTo = assignedTo,
                CreateTime = DateTime.Now.AddDays(-1),
                UpdateTime = DateTime.Now.AddMinutes(-30),
                Remark = "周度检查工单"
            };

            // 创建故障维修工单
            var order3 = new MaintenanceOrder
            {
                OrderCode = "MO003",
                OrderType = 2, // 故障维修
                EquipmentId = equipments[0].ResourceId,
                MaintenancePlanId = null,
                FaultDescription = "设备运行时有异常噪音",
                FaultCode = "E001",
                FaultLevel = 2, // 一般
                Priority = 7,
                Status = 2, // 已分配
                PlanStartTime = DateTime.Now.AddHours(1),
                PlanEndTime = DateTime.Now.AddHours(3),
                ActualStartTime = null,
                ActualEndTime = null,
                ReportBy = reportBy,
                AssignedTo = assignedTo,
                CreateTime = DateTime.Now.AddMinutes(-60),
                UpdateTime = DateTime.Now.AddMinutes(-30),
                Remark = "需要检查电机轴承"
            };

            // 创建紧急维修工单
            var order4 = new MaintenanceOrder
            {
                OrderCode = "MO004",
                OrderType = 3, // 紧急维修
                EquipmentId = equipments[2].ResourceId,
                MaintenancePlanId = null,
                FaultDescription = "测试台无法启动，电源指示灯不亮",
                FaultCode = "E002",
                FaultLevel = 3, // 严重
                Priority = 9,
                Status = 1, // 待处理
                PlanStartTime = DateTime.Now,
                PlanEndTime = DateTime.Now.AddHours(2),
                ActualStartTime = null,
                ActualEndTime = null,
                ReportBy = reportBy,
                AssignedTo = null,
                CreateTime = DateTime.Now.AddMinutes(-10),
                UpdateTime = null,
                Remark = "紧急处理，可能是电源问题"
            };

            context.Add(order1);
            context.Add(order2);
            context.Add(order3);
            context.Add(order4);
            await context.SaveChangesAsync();
            Debug.WriteLine($"维护工单数据添加完成，ID: {order1.Id}, {order2.Id}, {order3.Id}, {order4.Id}");
            
            // 为已完成的工单添加执行记录
            await InitializeMaintenanceExecutionsAsync(context, order1.Id, order2.Id);
        }
                /// <summary>
        /// 初始化设备参数记录数据
        /// </summary>
        private static async Task InitializeParameterLogsAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加设备参数记录数据");
            
            // 检查是否已存在参数记录数据
            if (await context.Set<EquipmentParameterLog>().AnyAsync())
            {
                Debug.WriteLine("设备参数记录数据已存在，跳过初始化");
                return;
            }
            
            // 获取设备数据
            var equipments = await context.Set<Equipment>().ToListAsync();
            if (equipments == null || equipments.Count == 0)
            {
                Debug.WriteLine("找不到设备数据，跳过参数记录初始化");
                return;
            }

            // 创建SMT贴片机的参数记录
            var smtLogs = new List<EquipmentParameterLog>();
            
            // 温度参数记录
            for (int i = 0; i < 24; i++)
            {
                double baseTemp = 45.0;
                double tempVariation = Math.Sin(i * Math.PI / 12) * 5.0; // 温度在一天内有波动
                double actualTemp = baseTemp + tempVariation;
                bool isAlarm = actualTemp > 55.0;
                
                var tempLog = new EquipmentParameterLog
                {
                    EquipmentId = equipments[0].ResourceId,
                    ParameterCode = "TEMP",
                    ParameterName = "工作温度",
                    ParameterValue = actualTemp.ToString("F1"),
                    Unit = "°C",
                    CollectTime = DateTime.Now.Date.AddHours(i),
                    IsAlarm = isAlarm,
                    AlarmLevel = isAlarm ? (byte)2 : null, // 警告级别
                    CreateTime = DateTime.Now.Date.AddHours(i)
                };
                smtLogs.Add(tempLog);
            }
            
            // 压力参数记录
            for (int i = 0; i < 24; i += 2)
            {
                double basePressure = 0.60;
                double pressureVariation = (new Random().NextDouble() - 0.5) * 0.1; // 随机波动
                double actualPressure = basePressure + pressureVariation;
                bool isAlarm = actualPressure < 0.5 || actualPressure > 0.7;
                
                var pressureLog = new EquipmentParameterLog
                {
                    EquipmentId = equipments[0].ResourceId,
                    ParameterCode = "PRES",
                    ParameterName = "气压",
                    ParameterValue = actualPressure.ToString("F2"),
                    Unit = "MPa",
                    CollectTime = DateTime.Now.Date.AddHours(i),
                    IsAlarm = isAlarm,
                    AlarmLevel = isAlarm ? (byte)1 : null, // 提示级别
                    CreateTime = DateTime.Now.Date.AddHours(i)
                };
                smtLogs.Add(pressureLog);
            }
            
            // 电流参数记录
            for (int i = 0; i < 24; i += 3)
            {
                double baseCurrent = 10.0;
                double currentVariation = (new Random().NextDouble() - 0.5) * 2.0; // 随机波动
                double actualCurrent = baseCurrent + currentVariation;
                bool isAlarm = actualCurrent > 12.0;
                
                var currentLog = new EquipmentParameterLog
                {
                    EquipmentId = equipments[0].ResourceId,
                    ParameterCode = "CURR",
                    ParameterName = "电流",
                    ParameterValue = actualCurrent.ToString("F1"),
                    Unit = "A",
                    CollectTime = DateTime.Now.Date.AddHours(i),
                    IsAlarm = isAlarm,
                    AlarmLevel = isAlarm ? (byte)3 : null, // 严重级别
                    CreateTime = DateTime.Now.Date.AddHours(i)
                };
                smtLogs.Add(currentLog);
            }

            // 创建焊接工位的参数记录
            var weldLogs = new List<EquipmentParameterLog>();
            
            // 温度参数记录
            for (int i = 0; i < 12; i += 1)
            {
                double baseTemp = 350.0;
                double tempVariation = (new Random().NextDouble() - 0.5) * 20.0; // 随机波动
                double actualTemp = baseTemp + tempVariation;
                bool isAlarm = actualTemp < 320.0 || actualTemp > 380.0;
                
                var tempLog = new EquipmentParameterLog
                {
                    EquipmentId = equipments[1].ResourceId,
                    ParameterCode = "TEMP",
                    ParameterName = "焊接温度",
                    ParameterValue = actualTemp.ToString("F1"),
                    Unit = "°C",
                    CollectTime = DateTime.Now.Date.AddHours(i * 2),
                    IsAlarm = isAlarm,
                    AlarmLevel = isAlarm ? (byte)2 : null, // 警告级别
                    CreateTime = DateTime.Now.Date.AddHours(i * 2)
                };
                weldLogs.Add(tempLog);
            }
            
            // 创建测试台的参数记录
            var testLogs = new List<EquipmentParameterLog>();
            
            // 电压参数记录
            for (int i = 0; i < 24; i += 4)
            {
                double baseVoltage = 220.0;
                double voltageVariation = (new Random().NextDouble() - 0.5) * 10.0; // 随机波动
                double actualVoltage = baseVoltage + voltageVariation;
                bool isAlarm = actualVoltage < 210.0 || actualVoltage > 230.0;
                
                var voltageLog = new EquipmentParameterLog
                {
                    EquipmentId = equipments[2].ResourceId,
                    ParameterCode = "VOLT",
                    ParameterName = "电源电压",
                    ParameterValue = actualVoltage.ToString("F1"),
                    Unit = "V",
                    CollectTime = DateTime.Now.Date.AddHours(i),
                    IsAlarm = isAlarm,
                    AlarmLevel = isAlarm ? (byte)1 : null, // 提示级别
                    CreateTime = DateTime.Now.Date.AddHours(i)
                };
                testLogs.Add(voltageLog);
            }

            // 添加所有参数记录
            foreach (var log in smtLogs.Concat(weldLogs).Concat(testLogs))
            {
                context.Add(log);
            }

            await context.SaveChangesAsync();
            Debug.WriteLine($"设备参数记录数据添加完成，共{smtLogs.Count + weldLogs.Count + testLogs.Count}条");
        }
        /// <summary>
        /// 初始化备件数据
        /// </summary>
        private static async Task InitializeSparesAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加备件数据");
            
            // 检查是否已存在备件数据
            if (await context.Set<Spare>().AnyAsync())
            {
                Debug.WriteLine("备件数据已存在，跳过初始化");
                return;
            }
            
            // 添加备件数据
            var spare1 = new Spare
            {
                SpareCode = "SP001",
                SpareName = "电机轴承",
                SpareType = 2, // 维修件
                Specification = "6205-2RS",
                Unit = "个",
                StockQuantity = 20,
                MinimumStock = 5,
                Price = 35.50m,
                Supplier = "轴承供应商A",
                LeadTime = 7,
                Location = "A区-01-01",
                IsActive = true,
                CreateTime = DateTime.Now,
                Remark = "SMT贴片机主轴承"
            };

            var spare2 = new Spare
            {
                SpareCode = "SP002",
                SpareName = "传感器",
                SpareType = 3, // 备用件
                Specification = "接近开关NPN NC",
                Unit = "个",
                StockQuantity = 10,
                MinimumStock = 3,
                Price = 120.00m,
                Supplier = "自动化元件供应商B",
                LeadTime = 14,
                Location = "A区-01-02",
                IsActive = true,
                CreateTime = DateTime.Now,
                Remark = "焊接设备用接近开关"
            };

            var spare3 = new Spare
            {
                SpareCode = "SP003",
                SpareName = "润滑油",
                SpareType = 1, // 易耗品
                Specification = "高温润滑脂 100g",
                Unit = "瓶",
                StockQuantity = 30,
                MinimumStock = 10,
                Price = 25.80m,
                Supplier = "润滑油供应商C",
                LeadTime = 5,
                Location = "B区-02-01",
                IsActive = true,
                CreateTime = DateTime.Now,
                Remark = "设备日常维护用"
            };

            var spare4 = new Spare
            {
                SpareCode = "SP004",
                SpareName = "控制板",
                SpareType = 3, // 备用件
                Specification = "PLC控制板XYZ-100",
                Unit = "块",
                StockQuantity = 2,
                MinimumStock = 1,
                Price = 1200.00m,
                Supplier = "自动化元件供应商D",
                LeadTime = 30,
                Location = "C区-03-01",
                IsActive = true,
                CreateTime = DateTime.Now,
                Remark = "测试设备主控板"
            };

            var spare5 = new Spare
            {
                SpareCode = "SP005",
                SpareName = "密封圈",
                SpareType = 1, // 易耗品
                Specification = "O型圈 20mm",
                Unit = "个",
                StockQuantity = 100,
                MinimumStock = 30,
                Price = 2.50m,
                Supplier = "密封件供应商E",
                LeadTime = 3,
                Location = "B区-02-02",
                IsActive = true,
                CreateTime = DateTime.Now,
                Remark = "液压系统密封用"
            };

            context.Add(spare1);
            context.Add(spare2);
            context.Add(spare3);
            context.Add(spare4);
            context.Add(spare5);
            await context.SaveChangesAsync();
            Debug.WriteLine($"备件数据添加完成，ID: {spare1.Id}, {spare2.Id}, {spare3.Id}, {spare4.Id}, {spare5.Id}");
        }

        /// <summary>
        /// 初始化设备维护计划数据
        /// </summary>
        private static async Task InitializeMaintenancePlansAsync(MesDbContext context)
        {
            Debug.WriteLine("开始添加设备维护计划数据");
            
            // 检查是否已存在维护计划数据
            if (await context.Set<EquipmentMaintenancePlan>().AnyAsync())
            {
                Debug.WriteLine("维护计划数据已存在，跳过初始化");
                return;
            }
            
            // 获取设备数据
            var equipments = await context.Set<Equipment>().ToListAsync();
            if (equipments == null || equipments.Count == 0)
            {
                Debug.WriteLine("找不到设备数据，跳过维护计划初始化");
                return;
            }
            
            // 获取管理员用户ID
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            int createBy = adminUser?.Id ?? 1;

            // 为SMT贴片机添加维护计划
            var plan1 = new EquipmentMaintenancePlan
            {
                PlanCode = "MP001",
                PlanName = "SMT贴片机日常保养",
                EquipmentId = equipments[0].ResourceId,
                MaintenanceType = 1, // 日常保养
                CycleType = 1, // 天
                CycleValue = 1,
                StandardTime = 30, // 30分钟
                LastExecuteDate = DateTime.Now.AddDays(-1),
                NextExecuteDate = DateTime.Now,
                Status = 1, // 启用
                CreateBy = createBy,
                CreateTime = DateTime.Now,
                Remark = "每日设备清洁和检查"
            };

            var plan2 = new EquipmentMaintenancePlan
            {
                PlanCode = "MP002",
                PlanName = "SMT贴片机月度维护",
                EquipmentId = equipments[0].ResourceId,
                MaintenanceType = 2, // 定期维护
                CycleType = 3, // 月
                CycleValue = 1,
                StandardTime = 120, // 120分钟
                LastExecuteDate = DateTime.Now.AddMonths(-1),
                NextExecuteDate = DateTime.Now.AddMonths(1),
                Status = 1, // 启用
                CreateBy = createBy,
                CreateTime = DateTime.Now,
                Remark = "月度全面检查和润滑"
            };

            // 为焊接工位添加维护计划
            var plan3 = new EquipmentMaintenancePlan
            {
                PlanCode = "MP003",
                PlanName = "焊接工位周检",
                EquipmentId = equipments[1].ResourceId,
                MaintenanceType = 2, // 定期维护
                CycleType = 2, // 周
                CycleValue = 1,
                StandardTime = 60, // 60分钟
                LastExecuteDate = DateTime.Now.AddDays(-7),
                NextExecuteDate = DateTime.Now,
                Status = 1, // 启用
                CreateBy = createBy,
                CreateTime = DateTime.Now,
                Remark = "焊接设备周度检查"
            };

            // 为测试台添加维护计划
            var plan4 = new EquipmentMaintenancePlan
            {
                PlanCode = "MP004",
                PlanName = "测试台季度校准",
                EquipmentId = equipments[2].ResourceId,
                MaintenanceType = 3, // 预防性维护
                CycleType = 4, // 季度
                CycleValue = 1,
                StandardTime = 240, // 240分钟
                LastExecuteDate = DateTime.Now.AddMonths(-3),
                NextExecuteDate = DateTime.Now,
                Status = 1, // 启用
                CreateBy = createBy,
                CreateTime = DateTime.Now,
                Remark = "测试设备季度校准和全面检查"
            };

            context.Add(plan1);
            context.Add(plan2);
            context.Add(plan3);
            context.Add(plan4);
            await context.SaveChangesAsync();
            Debug.WriteLine($"设备维护计划数据添加完成，ID: {plan1.Id}, {plan2.Id}, {plan3.Id}, {plan4.Id}");

            // 为维护计划添加维护项目
            await InitializeMaintenanceItemsAsync(context, plan1.Id, plan2.Id, plan3.Id, plan4.Id);
        }

        /// <summary>
        /// 初始化维护项目数据
        /// </summary>
        private static async Task InitializeMaintenanceItemsAsync(MesDbContext context, int plan1Id, int plan2Id, int plan3Id, int plan4Id)
        {
            Debug.WriteLine("开始添加维护项目数据");
            
            // 为日常保养计划添加维护项目
            var items1 = new List<MaintenanceItem>
            {
                new MaintenanceItem
                {
                    ItemCode = "MI001",
                    ItemName = "清洁设备表面",
                    MaintenancePlanId = plan1Id,
                    ItemType = 2, // 清洁
                    Method = "使用无尘布擦拭设备表面",
                    Tool = "无尘布",
                    SequenceNo = 1,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI002",
                    ItemName = "检查气压",
                    MaintenancePlanId = plan1Id,
                    ItemType = 1, // 检查
                    StandardValue = "0.6",
                    UpperLimit = "0.7",
                    LowerLimit = "0.5",
                    Unit = "MPa",
                    Method = "查看气压表读数",
                    SequenceNo = 2,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI003",
                    ItemName = "检查安全装置",
                    MaintenancePlanId = plan1Id,
                    ItemType = 1, // 检查
                    Method = "确认安全光栅和急停按钮功能正常",
                    SequenceNo = 3,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                }
            };

            // 为月度维护计划添加维护项目
            var items2 = new List<MaintenanceItem>
            {
                new MaintenanceItem
                {
                    ItemCode = "MI004",
                    ItemName = "润滑传动部件",
                    MaintenancePlanId = plan2Id,
                    ItemType = 3, // 润滑
                    Method = "在指定位置注入润滑油",
                    Tool = "润滑油枪",
                    SequenceNo = 1,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI005",
                    ItemName = "检查电机轴承",
                    MaintenancePlanId = plan2Id,
                    ItemType = 1, // 检查
                    Method = "听声音，检查是否有异常噪音",
                    SequenceNo = 2,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI006",
                    ItemName = "清洁内部零件",
                    MaintenancePlanId = plan2Id,
                    ItemType = 2, // 清洁
                    Method = "拆开外壳清洁内部零件",
                    Tool = "螺丝刀，无尘布",
                    SequenceNo = 3,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                }
            };

            // 为焊接工位周检添加维护项目
            var items3 = new List<MaintenanceItem>
            {
                new MaintenanceItem
                {
                    ItemCode = "MI007",
                    ItemName = "检查焊接头",
                    MaintenancePlanId = plan3Id,
                    ItemType = 1, // 检查
                    Method = "检查焊接头是否磨损",
                    SequenceNo = 1,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI008",
                    ItemName = "清洁焊接残渣",
                    MaintenancePlanId = plan3Id,
                    ItemType = 2, // 清洁
                    Method = "清除焊接残渣",
                    Tool = "刮刀，清洁剂",
                    SequenceNo = 2,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                }
            };

            // 为测试台季度校准添加维护项目
            var items4 = new List<MaintenanceItem>
            {
                new MaintenanceItem
                {
                    ItemCode = "MI009",
                    ItemName = "校准测量仪器",
                    MaintenancePlanId = plan4Id,
                    ItemType = 5, // 调整
                    Method = "使用标准件校准测量仪器",
                    Tool = "校准工具包",
                    SequenceNo = 1,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI010",
                    ItemName = "更换测试探针",
                    MaintenancePlanId = plan4Id,
                    ItemType = 4, // 更换
                    Method = "拆下旧探针，安装新探针",
                    Tool = "专用扳手",
                    SequenceNo = 2,
                    IsRequired = false,
                    CreateTime = DateTime.Now
                },
                new MaintenanceItem
                {
                    ItemCode = "MI011",
                    ItemName = "检查软件版本",
                    MaintenancePlanId = plan4Id,
                    ItemType = 1, // 检查
                    Method = "确认软件版本是否为最新",
                    SequenceNo = 3,
                    IsRequired = true,
                    CreateTime = DateTime.Now
                }
            };

            // 添加所有维护项目
            foreach (var item in items1.Concat(items2).Concat(items3).Concat(items4))
            {
                context.Add(item);
            }

            await context.SaveChangesAsync();
            Debug.WriteLine($"维护项目数据添加完成，共{items1.Count + items2.Count + items3.Count + items4.Count}条");
        }

        /// <summary>
        /// 初始化维护执行记录数据
        /// </summary>
        private static async Task InitializeMaintenanceExecutionsAsync(MesDbContext context, int completedOrderId, int inProgressOrderId)
        {
            Debug.WriteLine("开始添加维护执行记录数据");
            
            // 获取用户数据
            var users = await context.Users.ToListAsync();
            int executorId = users.FirstOrDefault(u => u.Username == "production")?.Id ?? 1;
            
            // 获取维护项目数据
            var maintenanceItems = await context.Set<MaintenanceItem>().ToListAsync();
            if (maintenanceItems == null || maintenanceItems.Count == 0)
            {
                Debug.WriteLine("找不到维护项目数据，跳过维护执行记录初始化");
                return;
            }
            
            // 获取备件数据
            var spares = await context.Set<Spare>().ToListAsync();
            if (spares == null || spares.Count == 0)
            {
                Debug.WriteLine("找不到备件数据，跳过备件使用记录初始化");
                return;
            }

            // 为已完成的工单添加执行记录
            var execution1 = new MaintenanceExecution
            {
                MaintenanceOrderId = completedOrderId,
                ExecutorId = executorId,
                StartTime = DateTime.Now.AddDays(-1).AddMinutes(10),
                EndTime = DateTime.Now.AddDays(-1).AddMinutes(40),
                LaborTime = 30,
                ExecutionResult = 1, // 正常
                ResultDescription = "设备保养完成，运行正常",
                ImageUrls = "[\"images/maintenance/20230601_001.jpg\", \"images/maintenance/20230601_002.jpg\"]",
                CreateTime = DateTime.Now.AddDays(-1).AddMinutes(10),
                UpdateTime = DateTime.Now.AddDays(-1).AddMinutes(40),
                Remark = "按计划完成日常保养"
            };

            // 为进行中的工单添加执行记录
            var execution2 = new MaintenanceExecution
            {
                MaintenanceOrderId = inProgressOrderId,
                ExecutorId = executorId,
                StartTime = DateTime.Now.AddMinutes(-30),
                EndTime = null,
                LaborTime = null,
                ExecutionResult = null,
                ResultDescription = null,
                CreateTime = DateTime.Now.AddMinutes(-30),
                Remark = "正在进行周度检查"
            };

            context.Add(execution1);
            context.Add(execution2);
            await context.SaveChangesAsync();
            Debug.WriteLine($"维护执行记录数据添加完成，ID: {execution1.Id}, {execution2.Id}");

            // 为已完成的执行记录添加维护项目执行记录
            var itemExecutions = new List<MaintenanceItemExecution>();
            
            // 为日常保养工单添加项目执行记录
            for (int i = 0; i < 3 && i < maintenanceItems.Count; i++)
            {
                var itemExecution = new MaintenanceItemExecution
                {
                    MaintenanceExecutionId = execution1.Id,
                    MaintenanceItemId = maintenanceItems[i].Id,
                    ItemName = maintenanceItems[i].ItemName,
                    ActualValue = i == 1 ? "0.62" : null, // 为气压检查项添加实际值
                    IsQualified = true,
                    ExecutionTime = DateTime.Now.AddDays(-1).AddMinutes(10 + i * 10),
                    ExecutorId = executorId,
                    Remark = "正常",
                    CreateTime = DateTime.Now.AddDays(-1).AddMinutes(10 + i * 10)
                };
                itemExecutions.Add(itemExecution);
            }
            
            // 为进行中的工单添加部分已完成的项目执行记录
            for (int i = 0; i < 1 && i < maintenanceItems.Count; i++)
            {
                var itemExecution = new MaintenanceItemExecution
                {
                    MaintenanceExecutionId = execution2.Id,
                    MaintenanceItemId = maintenanceItems[i + 6].Id, // 使用焊接工位的维护项目
                    ItemName = maintenanceItems[i + 6].ItemName,
                    IsQualified = true,
                    ExecutionTime = DateTime.Now.AddMinutes(-20),
                    ExecutorId = executorId,
                    Remark = "已检查",
                    CreateTime = DateTime.Now.AddMinutes(-20)
                };
                itemExecutions.Add(itemExecution);
            }

            foreach (var itemExecution in itemExecutions)
            {
                context.Add(itemExecution);
            }
            await context.SaveChangesAsync();
            Debug.WriteLine($"维护项目执行记录数据添加完成，共{itemExecutions.Count}条");

            // 添加备件使用记录
            var spareUsage1 = new SpareUsage
            {
                MaintenanceExecutionId = execution1.Id,
                SpareId = spares[2].Id, // 润滑油
                Quantity = 0.1m,
                UsageType = 3, // 消耗
                UsageTime = DateTime.Now.AddDays(-1).AddMinutes(25),
                OperatorId = executorId,
                CreateTime = DateTime.Now.AddDays(-1).AddMinutes(25),
                Remark = "用于润滑传动部件"
            };

            context.Add(spareUsage1);
            await context.SaveChangesAsync();
            Debug.WriteLine($"备件使用记录数据添加完成，ID: {spareUsage1.Id}");
        }

   
    }
} 