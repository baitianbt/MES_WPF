using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// OperationView.xaml 的交互逻辑
    /// </summary>
    public partial class OperationView : UserControl
    {
        public OperationView(OperationViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}