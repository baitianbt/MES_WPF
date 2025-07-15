using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MES_WPF.Services;
using MES_WPF.ViewModels.SystemManagement;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// UserManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class UserManagementView : UserControl
    {
        public UserManagementView(UserManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is UserManagementViewModel vm)
                {
                    _ = vm.RefreshUsersCommand.ExecuteAsync(null);
                }
            };
        }
    }
}