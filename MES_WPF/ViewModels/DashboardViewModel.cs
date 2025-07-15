using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using MES_WPF.Models;
using System.Collections.ObjectModel;

namespace MES_WPF.ViewModels
{
    /// <summary>
    /// 仪表盘视图模型
    /// </summary>
    public partial class DashboardViewModel : ObservableObject
    {
        private ChartValues<double> _productionTrendData;
        /// <summary>
        /// 生产趋势数据
        /// </summary>
        public ChartValues<double> ProductionTrendData
        {
            get => _productionTrendData;
            set => SetProperty(ref _productionTrendData, value);
        }

        private ObservableCollection<string> _productionTrendLabels;
        /// <summary>
        /// 生产趋势标签
        /// </summary>
        public ObservableCollection<string> ProductionTrendLabels
        {
            get => _productionTrendLabels;
            set => SetProperty(ref _productionTrendLabels, value);
        }

        private SeriesCollection _productTypeData;
        /// <summary>
        /// 产品类型数据
        /// </summary>
        public SeriesCollection ProductTypeData
        {
            get => _productTypeData;
            set => SetProperty(ref _productTypeData, value);
        }

        private ObservableCollection<TaskItem> _tasks;
        /// <summary>
        /// 任务列表
        /// </summary>
        public ObservableCollection<TaskItem> Tasks
        {
            get => _tasks;
            set => SetProperty(ref _tasks, value);
        }

        private ObservableCollection<NotificationItem> _notifications;
        /// <summary>
        /// 通知列表
        /// </summary>
        public ObservableCollection<NotificationItem> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public DashboardViewModel()
        {
            // 初始化集合
            ProductionTrendData = new ChartValues<double>();
            ProductionTrendLabels = new ObservableCollection<string>();
            ProductTypeData = new SeriesCollection();
            Tasks = new ObservableCollection<TaskItem>();
            Notifications = new ObservableCollection<NotificationItem>();
        }
    }
} 