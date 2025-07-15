using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// EquipmentView.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentView : UserControl
    {
        public EquipmentView(EquipmentViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}