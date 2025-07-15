using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using MES_WPF.Helpers;
using MES_WPF.Models;
using MES_WPF.Services;
using MES_WPF.ViewModels;
using MES_WPF.Views;
using MES_WPF.Views.SystemManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MES_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly MenuViewModel _menuViewModel;
        
        // 左侧菜单是否展开
        private bool _isMenuExpanded = true;
        
        public MainWindow(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _menuViewModel = App.GetService<MenuViewModel>();
            
            InitializeComponent();
            DataContext = _menuViewModel;
            
            // 设置导航服务的内容控件
            if (_navigationService is NavigationService navService)
            {
                Loaded += (s, e) =>
                {
                    if (FindName("ContentControl") is ContentControl contentControl)
                    {
                        navService.SetContentControl(contentControl);
                        
                        // 加载DashboardView
                        var dashboardView = App.GetService<DashboardView>();
                        dashboardView.DataContext = App.GetService<DashboardViewModel>();
                        _menuViewModel.MainContent = dashboardView;
                    }
                };
            }
            
            // 初始化数据
            InitializeData();
        }

        private void InitializeData()
        {
            // 初始化生产趋势数据
            var productionTrendData = new ChartValues<double> { 110, 120, 105, 130, 115, 200, 205 };
            var productionTrendLabels = new List<string> { "5-14", "5-15", "5-16", "5-17", "5-18", "5-19", "5-20" };

            // 初始化产品类型数据
            var productTypeData = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "A型产品",
                    Values = new ChartValues<double> { 25 },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromRgb(63, 81, 181))
                },
                new PieSeries
                {
                    Title = "B型产品",
                    Values = new ChartValues<double> { 20 },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromRgb(156, 204, 101))
                },
                new PieSeries
                {
                    Title = "C型产品",
                    Values = new ChartValues<double> { 30 },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromRgb(255, 205, 86))
                },
                new PieSeries
                {
                    Title = "D型产品",
                    Values = new ChartValues<double> { 15 },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromRgb(239, 83, 80))
                },
                new PieSeries
                {
                    Title = "其他",
                    Values = new ChartValues<double> { 10 },
                    DataLabels = true,
                    Fill = new SolidColorBrush(Color.FromRgb(77, 208, 225))
                }
            };
            
            // 初始化任务列表
            var tasks = new List<TaskItem>();
            tasks.Add(new TaskItem 
            { 
                TaskName = "生产计划P10023的生产任务",
                PlanDate = "2024-05-20",
                Priority = "高",
                PriorityColor = "#F44336"
            });
            
            tasks.Add(new TaskItem 
            { 
                TaskName = "设备A102维护保养",
                PlanDate = "2024-05-21",
                Priority = "中",
                PriorityColor = "#FF9800"
            });
            
            tasks.Add(new TaskItem 
            { 
                TaskName = "原料RH403入库检验",
                PlanDate = "2024-05-20",
                Priority = "高",
                PriorityColor = "#F44336"
            });
            
            // 初始化通知列表
            var notifications = new List<NotificationItem>();
            
            notifications.Add(new NotificationItem
            {
                IconKind = "Alert",
                IconBackground = "#F44336",
                Title = "设备D15检测到异常，请及时处理",
                Time = "10分钟前"
            });
            
            notifications.Add(new NotificationItem
            {
                IconKind = "Check",
                IconBackground = "#4CAF50",
                Title = "生产计划P10023已完成",
                Time = "20分钟前"
            });
            
            notifications.Add(new NotificationItem
            {
                IconKind = "Information",
                IconBackground = "#2196F3",
                Title = "新的生产任务已分配",
                Time = "1小时前"
            });
            
            notifications.Add(new NotificationItem
            {
                IconKind = "Alert",
                IconBackground = "#F44336",
                Title = "质量检测发现不合格品",
                Time = "2小时前"
            });
            
            // 将数据传递给DashboardView
            if (_menuViewModel.MainContent is DashboardView dashboardView)
            {
                if (dashboardView.DataContext is DashboardViewModel dashboardViewModel)
                {
                    dashboardViewModel.ProductionTrendData = new ChartValues<double>(productionTrendData);
                    dashboardViewModel.ProductionTrendLabels = new ObservableCollection<string>(productionTrendLabels);
                    dashboardViewModel.ProductTypeData = productTypeData;
                    dashboardViewModel.Tasks = new ObservableCollection<TaskItem>(tasks);
                    dashboardViewModel.Notifications = new ObservableCollection<NotificationItem>(notifications);
                }
            }
        }
        
        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isMenuExpanded = !_isMenuExpanded;
            
            var columnDefinition = (ColumnDefinition)((Grid)Content).ColumnDefinitions[0];
            
            if (_isMenuExpanded)
            {
                columnDefinition.Width = new GridLength(220);
            }
            else
            {
                columnDefinition.Width = new GridLength(60);
            }
        }
        
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // 使用MVVM模式，不再需要在代码后台处理选择事件
            // 选择事件由MenuViewModel处理
        }
        
        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem && treeViewItem.DataContext is MenuItemModel menuItem)
            {
                // 阻止事件继续冒泡，防止重复触发
                e.Handled = true;
                
                // 触发菜单项选择命令
                if (_menuViewModel.SelectMenuItemCommand.CanExecute(menuItem))
                {
                    _menuViewModel.SelectMenuItemCommand.Execute(menuItem);
                }
                
                // 手动更新TreeView，确保UI更新
                if (FindName("MenuTreeView") is TreeView menuTreeView)
                {
                    menuTreeView.UpdateLayout();
                }
            }
        }

        private void TreeViewItem_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem && treeViewItem.DataContext is MenuItemModel menuItem)
            {
                // 阻止事件继续冒泡，防止重复触发
                e.Handled = true;
                
                // 选中当前项
                treeViewItem.IsSelected = true;
                
                // 如果有子菜单，则切换展开状态
                if (menuItem.HasSubItems && menuItem.ExpandCommand.CanExecute(null))
                {
                    menuItem.ExpandCommand.Execute(null);
                }
            }
        }
    }
    
    public class TaskItem
    {
        public string TaskName { get; set; }
        public string PlanDate { get; set; }
        public string Priority { get; set; }
        public string PriorityColor { get; set; }
    }
    
    public class NotificationItem
    {
        public string IconKind { get; set; }
        public string IconBackground { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
    }
} 