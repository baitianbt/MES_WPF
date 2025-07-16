using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using MES_WPF.Models;
using MES_WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MES_WPF.ViewModels
{
    /// <summary>
    /// 菜单视图模型
    /// </summary>
    public partial class MenuViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        /// <summary>
        /// 菜单项集合
        /// </summary>
        public ObservableCollection<MenuItemModel> MenuItems { get; } = new ObservableCollection<MenuItemModel>();

        private MenuItemModel _selectedMenuItem;
        /// <summary>
        /// 当前选中的菜单项
        /// </summary>
        public MenuItemModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set => SetProperty(ref _selectedMenuItem, value);
        }

        private string _currentMenuTitle = "首页";
        /// <summary>
        /// 当前菜单标题
        /// </summary>
        public string CurrentMenuTitle
        {
            get => _currentMenuTitle;
            set => SetProperty(ref _currentMenuTitle, value);
        }

        private string _currentSubMenuTitle = "系统首页";
        /// <summary>
        /// 当前子菜单标题
        /// </summary>
        public string CurrentSubMenuTitle
        {
            get => _currentSubMenuTitle;
            set => SetProperty(ref _currentSubMenuTitle, value);
        }

        private object _mainContent;
        /// <summary>
        /// 主内容区
        /// </summary>
        public object MainContent
        {
            get => _mainContent;
            set => SetProperty(ref _mainContent, value);
        }

        /// <summary>
        /// 菜单项选择命令
        /// </summary>
        public ICommand SelectMenuItemCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 初始化命令
            SelectMenuItemCommand = new RelayCommand<MenuItemModel>(OnSelectMenuItem);
            
            // 初始化菜单数据
            InitializeMenuItems();
        }

        /// <summary>
        /// 初始化菜单项
        /// </summary>
        private void InitializeMenuItems()
        {
            // 系统首页
            var dashboardItem = new MenuItemModel
            {
                Title = "系统首页",
                Icon = PackIconKind.ViewDashboard,
                ViewName = "DashboardView",
                IsSelected = true
            };
            dashboardItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(dashboardItem));
            MenuItems.Add(dashboardItem);

            // 基础数据
            var BasicItem = new MenuItemModel
            {
                Title = "基础信息",
                Icon = PackIconKind.Factory,
                ViewName = "ProductionView"
            };
            BasicItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(BasicItem));
            BasicItem.SubItems.Add(new MenuItemModel { Title = "BOM管理", ViewName = "BOMView" });
            BasicItem.SubItems.Add(new MenuItemModel { Title = "设备管理", ViewName = "EquipmentView" });
            BasicItem.SubItems.Add(new MenuItemModel { Title = "操作管理", ViewName = "OperationView" });
            BasicItem.SubItems.Add(new MenuItemModel { Title = "工序管理", ViewName = "ProcessRouteView" });
            BasicItem.SubItems.Add(new MenuItemModel { Title = "产品管理", ViewName = "ProductView" });
            BasicItem.SubItems.Add(new MenuItemModel { Title = "资源信息管理", ViewName = "ResourceView" });
            MenuItems.Add(BasicItem);

            // 生产管理
            var productionItem = new MenuItemModel
            {
                Title = "生产管理",
                Icon = PackIconKind.Factory,
                ViewName = "ProductionView"
            };
            productionItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(productionItem));
            productionItem.SubItems.Add(new MenuItemModel { Title = "生产计划", ViewName = "ProductionPlanView" });
            productionItem.SubItems.Add(new MenuItemModel { Title = "生产执行", ViewName = "ProductionExecutionView" });
            productionItem.SubItems.Add(new MenuItemModel { Title = "生产报表", ViewName = "ProductionReportView" });
            MenuItems.Add(productionItem);

            // 设备管理
            var equipmentItem = new MenuItemModel
            {
                Title = "设备管理",
                Icon = PackIconKind.Tools,
                ViewName = "EquipmentView"
            };
            equipmentItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(equipmentItem));
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "维护执行", ViewName = "MaintenanceExecutionView" });
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "维护项目", ViewName = "MaintenanceItemView" });
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "维护订单", ViewName = "MaintenanceOrderView" });
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "设备维护计划", ViewName = "MaintenancePlanView" });
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "参数日志", ViewName = "MaintenanceItemView" });
            equipmentItem.SubItems.Add(new MenuItemModel { Title = "备件仓", ViewName = "SpareView" });
            MenuItems.Add(equipmentItem);

            // 物料管理
            var materialsItem = new MenuItemModel
            {
                Title = "物料管理",
                Icon = PackIconKind.Package,
                ViewName = "MaterialsView"
            };
            materialsItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(materialsItem));
            materialsItem.SubItems.Add(new MenuItemModel { Title = "物料库存", ViewName = "MaterialsInventoryView" });
            materialsItem.SubItems.Add(new MenuItemModel { Title = "物料采购", ViewName = "MaterialsPurchaseView" });
            materialsItem.SubItems.Add(new MenuItemModel { Title = "物料出入库", ViewName = "MaterialsInOutView" });
            MenuItems.Add(materialsItem);

            // 质量管理
            var qualityItem = new MenuItemModel
            {
                Title = "质量管理",
                Icon = PackIconKind.CheckCircle,
                ViewName = "QualityView"
            };
            qualityItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(qualityItem));
            qualityItem.SubItems.Add(new MenuItemModel { Title = "质量检验", ViewName = "QualityInspectionView" });
            qualityItem.SubItems.Add(new MenuItemModel { Title = "不良品管理", ViewName = "DefectiveProductView" });
            qualityItem.SubItems.Add(new MenuItemModel { Title = "质量报表", ViewName = "QualityReportView" });
            MenuItems.Add(qualityItem);

            // 系统管理
            var systemItem = new MenuItemModel
            {
                Title = "系统管理",
                Icon = PackIconKind.Cog,
                ViewName = "SystemView"
            };
            systemItem.ExpandCommand = new RelayCommand<object>(_ => ToggleMenuExpand(systemItem));
            systemItem.SubItems.Add(new MenuItemModel { Title = "用户管理", ViewName = "UserManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "角色管理", ViewName = "RoleManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "员工管理", ViewName = "EmployeeManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "部门管理", ViewName = "DepartmentManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "权限设置", ViewName = "PermissionManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "系统设置", ViewName = "SystemConfigManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "数据字典", ViewName = "DictionaryManagementView" });
            systemItem.SubItems.Add(new MenuItemModel { Title = "操作日志", ViewName = "OperationLogManagementView" });
            MenuItems.Add(systemItem);

            // 设置默认选中项
            SelectedMenuItem = dashboardItem;
        }

        /// <summary>
        /// 切换菜单展开状态
        /// </summary>
        private void ToggleMenuExpand(MenuItemModel menuItem)
        {
            if (menuItem == null) return;

            // 切换展开状态
            menuItem.IsExpanded = !menuItem.IsExpanded;
            
            // 如果是展开，则折叠其他菜单项
            if (menuItem.IsExpanded)
            {
                foreach (var item in MenuItems)
                {
                    if (item != menuItem && item.IsExpanded)
                    {
                        item.IsExpanded = false;
                    }
                }
            }
            
            // 手动触发属性更改通知
            OnPropertyChanged(nameof(MenuItems));
        }

        /// <summary>
        /// 菜单项选择处理
        /// </summary>
        private void OnSelectMenuItem(MenuItemModel menuItem)
        {
            if (menuItem == null) return;
            
            // 更新选中状态
            foreach (var item in MenuItems)
            {
                item.IsSelected = (item == menuItem);
                
                foreach (var subItem in item.SubItems)
                {
                    if (subItem == menuItem)
                    {
                        subItem.IsSelected = true;
                        // 确保父菜单展开
                        item.IsExpanded = true;
                    }
                    else
                    {
                        subItem.IsSelected = false;
                    }
                }
            }

            // 设置当前菜单标题
            if (menuItem.SubItems.Count == 0)
            {
                // 查找父菜单
                var parentItem = MenuItems.FirstOrDefault(item => item.SubItems.Contains(menuItem));
                if (parentItem != null)
                {
                    CurrentMenuTitle = parentItem.Title;
                    CurrentSubMenuTitle = menuItem.Title;
                }
                else
                {
                    CurrentMenuTitle = menuItem.Title;
                    CurrentSubMenuTitle = string.Empty;
                }
            }
            else
            {
                CurrentMenuTitle = menuItem.Title;
                CurrentSubMenuTitle = string.Empty;
            }

            // 导航到对应视图
            if (!string.IsNullOrEmpty(menuItem.ViewName))
            {
                _navigationService.NavigateTo(menuItem.ViewName);
            }

            // 更新选中项
            SelectedMenuItem = menuItem;
        }
    }
} 