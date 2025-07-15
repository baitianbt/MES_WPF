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
    /// DepartmentManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class DepartmentManagementView : UserControl
    {
        public DepartmentManagementView(DepartmentManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is DepartmentManagementViewModel vm)
                {
                    _ = vm.RefreshDepartmentsCommand.ExecuteAsync(null);
                }
            };
        }
    }
} 