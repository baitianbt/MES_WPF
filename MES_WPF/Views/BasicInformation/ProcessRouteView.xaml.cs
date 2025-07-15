using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// ProcessRouteView.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessRouteView : UserControl
    {
        public ProcessRouteView(ProcessRouteViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
} 