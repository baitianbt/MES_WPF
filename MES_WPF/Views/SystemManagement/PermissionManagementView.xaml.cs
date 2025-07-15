using System;
using System.Windows.Controls;
using MES_WPF.ViewModels.SystemManagement;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// PermissionManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class PermissionManagementView : UserControl
    {
        public PermissionManagementView(PermissionManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is PermissionManagementViewModel vm)
                {
                    _ = vm.RefreshPermissionsCommand.ExecuteAsync(null);
                }
            };
        }
    }
} 