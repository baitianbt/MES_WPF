using System;
using System.Windows.Controls;
using MES_WPF.ViewModels.EquipmentManagement;

namespace MES_WPF.Views.EquipmentManagement
{
    /// <summary>
    /// MaintenanceItemView.xaml 的交互逻辑
    /// </summary>
    public partial class MaintenanceItemView : UserControl
    {
        public MaintenanceItemView(MaintenanceItemViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // 加载完成后刷新数据
            Loaded += (s, e) => 
            {
                if (DataContext is MaintenanceItemViewModel vm)
                {
                    _ = vm.RefreshItemsCommand.ExecuteAsync(null);
                }
            };
        }
    }
} 