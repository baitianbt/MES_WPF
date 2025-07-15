using MES_WPF.ViewModels.SystemManagement;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// SystemConfigManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class SystemConfigManagementView : UserControl
    {
        public SystemConfigManagementView(SystemConfigManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // 加载完成后刷新数据
            Loaded += (s, e) =>
            {
                if (DataContext is SystemConfigManagementViewModel vm)
                {
                    _ = vm.RefreshConfigsCommand.ExecuteAsync(null);
                }
            };
        }

    }
} 