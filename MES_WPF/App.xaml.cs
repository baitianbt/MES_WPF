using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MES_WPF.Services;
using MES_WPF.ViewModels;
using MES_WPF.ViewModels.SystemManagement;
using MES_WPF.ViewModels.BasicInformation;
using MES_WPF.Views;
using MES_WPF.Views.SystemManagement;
using MES_WPF.Views.BasicInformation;
using MES_WPF.Core.Services;
using MES_WPF.Core.Services.SystemManagement;
using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Data;

using Microsoft.EntityFrameworkCore;
using MaterialDesignThemes.Wpf;
using MES_WPF.Data.Repositories.SystemManagement;
using MES_WPF.Data.Repositories.BasicInformation;
using System.Threading.Tasks;
using MES_WPF.Core.Services.EquipmentManagement;
using MES_WPF.Data.Repositories.EquipmentManagement;
using MES_WPF.ViewModels.EquipmentManagement;
using MES_WPF.Views.EquipmentManagement;

namespace MES_WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static IServiceProvider _serviceProvider;
        private Window _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 配置依赖注入
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            
            try
            {
                // 初始化数据库
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<MesDbContext>();
                    // 确保数据库存在
                    dbContext.Database.EnsureCreated();
                    // 等待初始化完成
                    Task.Run(async () => await DbInitializer.InitializeAsync(dbContext)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库初始化失败: {ex.Message}\n{ex.StackTrace}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // 显示登录窗口
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            loginView.DataContext = loginViewModel;
            // 登录成功，显示主窗口
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            Application.Current.MainWindow = mainWindow;
            _mainWindow = mainWindow; // 保持引用，防止垃圾回收
            // 创建包含LoginView的窗口
            var loginWindow = new Window
            {
                Title = "登录 - MES系统",
                Content = loginView,
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            // 添加登录完成事件处理
            bool loginSuccess = false;
            loginViewModel.LoginCompleted += (sender, success) =>
            {
                loginSuccess = success;
                loginWindow.DialogResult = success;
                
            };

            // 显示登录窗口
            var result = loginWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
               
                mainWindow.Show();
            }
            else
            {
                // 登录失败或取消，退出应用程序
                Shutdown();
            }
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // 注册数据库上下文
            services.AddDbContext<MesDbContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mes.db")}"));

            // 注册通用服务
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // 注册所有仓储
            // 用户相关仓储
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IPermissionRepository, PermissionRepository>();
            services.AddSingleton<IUserRoleRepository, UserRoleRepository>();
            services.AddSingleton<IRolePermissionRepository, RolePermissionRepository>();

            // 部门和员工仓储
            services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();

            // 字典仓储
            services.AddSingleton<IDictionaryRepository, DictionaryRepository>();
            services.AddSingleton<IDictionaryItemRepository, DictionaryItemRepository>();

            // 系统配置和日志仓储
            services.AddSingleton<ISystemConfigRepository, SystemConfigRepository>();
            services.AddSingleton<IOperationLogRepository, OperationLogRepository>();

            // 基础信息模块仓储
            services.AddSingleton<IBOMRepository, BOMRepository>();
            services.AddSingleton<IBOMItemRepository, BOMItemRepository>();
            services.AddSingleton<IEquipmentRepository, EquipmentRepository>();
            services.AddSingleton<IOperationRepository, OperationRepository>();
            services.AddSingleton<IProcessRouteRepository, ProcessRouteRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IResourceRepository, ResourceRepository>();
            services.AddSingleton<IRouteStepRepository, RouteStepRepository>();

            // 设备管理模块仓储
            services.AddSingleton<IMaintenanceOrderRepository, MaintenanceOrderRepository>();
            services.AddSingleton<IMaintenanceExecutionRepository, MaintenanceExecutionRepository>();
            services.AddSingleton<IMaintenanceItemExecutionRepository, MaintenanceItemExecutionRepository>();
            services.AddSingleton<IEquipmentMaintenancePlanRepository, EquipmentMaintenancePlanRepository>();
            services.AddSingleton<IEquipmentParameterLogRepository, EquipmentParameterLogRepository>();
            services.AddSingleton<IMaintenanceItemRepository, MaintenanceItemRepository>();
            services.AddSingleton<ISpareRepository, SpareRepository>();
            services.AddSingleton<ISpareUsageRepository, SpareUsageRepository>();

            // 注册所有服务
            // 用户相关服务
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IPermissionService, PermissionService>();

            // 部门和员工服务
            services.AddSingleton<IDepartmentService, DepartmentService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();

            // 字典服务
            services.AddSingleton<IDictionaryService, DictionaryService>();
            services.AddSingleton<IDictionaryItemService, DictionaryItemService>();

            // 系统配置和日志服务
            services.AddSingleton<ISystemConfigService, SystemConfigService>();
            services.AddSingleton<IOperationLogService, OperationLogService>();

            // 基础信息模块服务
            services.AddSingleton<IBOMService, BOMService>();
            services.AddSingleton<IBOMItemService, BOMItemService>();
            services.AddSingleton<IEquipmentService, EquipmentService>();
            services.AddSingleton<IOperationService, OperationService>();
            services.AddSingleton<IProcessRouteService, ProcessRouteService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IResourceService, ResourceService>();
            services.AddSingleton<IRouteStepService, RouteStepService>();



            // 设备管理模块服务
            services.AddSingleton<IMaintenanceOrderService, MaintenanceOrderService>();
            services.AddSingleton<IMaintenanceExecutionService, MaintenanceExecutionService>();
            services.AddSingleton<IMaintenanceItemExecutionService, MaintenanceItemExecutionService>();
            services.AddSingleton<IEquipmentMaintenancePlanService, EquipmentMaintenancePlanService>();
            services.AddSingleton<IMaintenanceItemService, MaintenanceItemService>();
            services.AddSingleton<IEquipmentParameterLogService, EquipmentParameterLogService>();
            services.AddSingleton<ISpareService, SpareService>();
            services.AddSingleton<ISpareUsageService, SpareUsageService>();
            // 注册视图模型
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<UserManagementViewModel>();
            services.AddSingleton<DepartmentManagementViewModel>();
            services.AddSingleton<DictionaryManagementViewModel>();
            services.AddSingleton<EmployeeManagementViewModel>();
            services.AddSingleton<OperationLogManagementViewModel>();
            services.AddSingleton<PermissionManagementViewModel>();
            services.AddSingleton<RoleManagementViewModel>();
            services.AddSingleton<SystemConfigManagementViewModel>();

            // 设备管理模块视图模型
            services.AddSingleton<MaintenanceExecutionViewModel>();
            services.AddSingleton<MaintenanceItemViewModel>();
            services.AddSingleton<MaintenanceOrderViewModel>();
            services.AddSingleton<MaintenancePlanViewModel>();
            services.AddSingleton<ParameterLogViewModel>();
            services.AddSingleton<SpareViewModel>();

            // 基础信息模块视图模型
            services.AddSingleton<BOMViewModel>();
            services.AddSingleton<EquipmentViewModel>();
            services.AddSingleton<OperationViewModel>();
            services.AddSingleton<ProcessRouteViewModel>();
            services.AddSingleton<ProductViewModel>();
            services.AddSingleton<ResourceViewModel>();


            // 设备管理模块视图
            services.AddSingleton<MaintenanceExecutionView>();
            services.AddSingleton<MaintenanceItemView>();
            services.AddSingleton<MaintenanceOrderView>();
            services.AddSingleton<MaintenancePlanView>();
            services.AddSingleton<ParameterLogView>();
            services.AddSingleton<SpareView>();


            // 注册视图
            services.AddSingleton<DepartmentManagementView>();
            services.AddSingleton<DictionaryManagementView>();
            services.AddSingleton<EmployeeManagementView>();
            services.AddSingleton<OperationLogManagementView>();
            services.AddSingleton<PermissionManagementView>();
            services.AddSingleton<RoleManagementView>();
            services.AddSingleton<SystemConfigManagementView>();

            // 基础信息模块视图
            services.AddSingleton<BOMView>();
            services.AddSingleton<EquipmentView>();
            services.AddSingleton<OperationView>();
            services.AddSingleton<ProcessRouteView>();
            services.AddSingleton<ProductView>();
            services.AddSingleton<ResourceView>();
        
            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<UserManagementView>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
} 