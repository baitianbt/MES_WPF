using System;
using System.Windows.Controls;
using MES_WPF.ViewModels.SystemManagement;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// RoleManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class RoleManagementView : UserControl
    {
        public RoleManagementView(RoleManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is RoleManagementViewModel vm)
                {
                    _ = vm.RefreshRolesCommand.ExecuteAsync(null);
                }
            };
        }
    }
} 