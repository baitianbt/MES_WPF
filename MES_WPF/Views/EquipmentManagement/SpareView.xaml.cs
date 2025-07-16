using MES_WPF.ViewModels.EquipmentManagement;
using System.Windows.Controls;

namespace MES_WPF.Views.EquipmentManagement
{
    /// <summary>
    /// SpareView.xaml 的交互逻辑
    /// </summary>
    public partial class SpareView : UserControl
    {
        public SpareView(SpareViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
} 