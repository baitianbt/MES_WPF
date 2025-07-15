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
    }
} 