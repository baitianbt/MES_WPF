using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using MES_WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

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
            
            // 生成模拟数据
            GenerateProductionTrendData();
            GenerateProductTypeData();
            GenerateTaskData();
            GenerateNotificationData();
        }
        
        /// <summary>
        /// 生成生产趋势模拟数据
        /// </summary>
        private void GenerateProductionTrendData()
        {
            // 生成近7天的日期标签
            DateTime now = DateTime.Now;
            for (int i = 6; i >= 0; i--)
            {
                DateTime date = now.AddDays(-i);
                ProductionTrendLabels.Add(date.ToString("MM-dd"));
            }
            
            // 生成生产数据
            Random random = new Random();
            for (int i = 0; i < 7; i++)
            {
                ProductionTrendData.Add(random.Next(150, 250));
            }
        }
        
        /// <summary>
        /// 生成产品类型饼图模拟数据
        /// </summary>
        private void GenerateProductTypeData()
        {
            ProductTypeData.Add(new PieSeries
            {
                Title = "A型产品",
                Values = new ChartValues<double> { 35 },
                DataLabels = true,
                LabelPoint = point => $"A型: {point.Y}%"
            });
            
            ProductTypeData.Add(new PieSeries
            {
                Title = "B型产品",
                Values = new ChartValues<double> { 25 },
                DataLabels = true,
                LabelPoint = point => $"B型: {point.Y}%"
            });
            
            ProductTypeData.Add(new PieSeries
            {
                Title = "C型产品",
                Values = new ChartValues<double> { 20 },
                DataLabels = true,
                LabelPoint = point => $"C型: {point.Y}%"
            });
            
            ProductTypeData.Add(new PieSeries
            {
                Title = "D型产品",
                Values = new ChartValues<double> { 15 },
                DataLabels = true,
                LabelPoint = point => $"D型: {point.Y}%"
            });
            
            ProductTypeData.Add(new PieSeries
            {
                Title = "其他",
                Values = new ChartValues<double> { 5 },
                DataLabels = true,
                LabelPoint = point => $"其他: {point.Y}%"
            });
        }
        
        /// <summary>
        /// 生成任务列表模拟数据
        /// </summary>
        private void GenerateTaskData()
        {
            Tasks.Add(new TaskItem 
            { 
                TaskName = "A型产品生产计划审批", 
                PlanDate = DateTime.Now.ToString("yyyy-MM-dd"),
                Priority = "高",
                PriorityColor = "#F44336" // 红色
            });
            
            Tasks.Add(new TaskItem 
            { 
                TaskName = "B型产品质检报告确认", 
                PlanDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                Priority = "中",
                PriorityColor = "#FF9800" // 橙色
            });
            
            Tasks.Add(new TaskItem 
            { 
                TaskName = "设备维护计划制定", 
                PlanDate = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"),
                Priority = "低",
                PriorityColor = "#4CAF50" // 绿色
            });
            
            Tasks.Add(new TaskItem 
            { 
                TaskName = "原材料采购申请审批", 
                PlanDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                Priority = "高",
                PriorityColor = "#F44336" // 红色
            });
        }
        
        /// <summary>
        /// 生成通知列表模拟数据
        /// </summary>
        private void GenerateNotificationData()
        {
            Notifications.Add(new NotificationItem
            {
                Title = "系统更新通知",
                Time = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm"),
                IconKind = "Bell",
                IconBackground = "#2196F3" // 蓝色
            });
            
            Notifications.Add(new NotificationItem
            {
                Title = "设备维护提醒",
                Time = DateTime.Now.AddHours(-3).ToString("yyyy-MM-dd HH:mm"),
                IconKind = "Tools",
                IconBackground = "#FF9800" // 橙色
            });
            
            Notifications.Add(new NotificationItem
            {
                Title = "生产计划已完成",
                Time = DateTime.Now.AddHours(-5).ToString("yyyy-MM-dd HH:mm"),
                IconKind = "CheckCircle",
                IconBackground = "#4CAF50" // 绿色
            });
            
            Notifications.Add(new NotificationItem
            {
                Title = "质检异常警报",
                Time = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm"),
                IconKind = "Alert",
                IconBackground = "#F44336" // 红色
            });
        }
    }
} 