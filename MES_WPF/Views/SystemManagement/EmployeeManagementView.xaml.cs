using System;
using System.Windows.Controls;
using MES_WPF.ViewModels.SystemManagement;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// EmployeeManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class EmployeeManagementView : UserControl
    {
        public EmployeeManagementView(EmployeeManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is EmployeeManagementViewModel vm)
                {
                    _ = vm.RefreshEmployeesCommand.ExecuteAsync(null);
                }
            };
        }
    }
} 