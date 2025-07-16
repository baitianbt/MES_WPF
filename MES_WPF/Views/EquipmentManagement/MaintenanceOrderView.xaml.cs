using System;
using System.Windows.Controls;
using MES_WPF.ViewModels.EquipmentManagement;

namespace MES_WPF.Views.EquipmentManagement
{
    /// <summary>
    /// MaintenanceOrderView.xaml 的交互逻辑
    /// </summary>
    public partial class MaintenanceOrderView : UserControl
    {
        public MaintenanceOrderView(MaintenanceOrderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is MaintenanceOrderViewModel vm)
                {
                    _ = vm.RefreshOrdersCommand.ExecuteAsync(null);
                }
            };
        }
    }
}