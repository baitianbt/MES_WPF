using MES_WPF.ViewModels.EquipmentManagement;
using System.Windows.Controls;

namespace MES_WPF.Views.EquipmentManagement
{
    /// <summary>
    /// MaintenanceExecutionView.xaml 的交互逻辑
    /// </summary>
    public partial class MaintenanceExecutionView : UserControl
    {
        public MaintenanceExecutionView(MaintenanceExecutionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}