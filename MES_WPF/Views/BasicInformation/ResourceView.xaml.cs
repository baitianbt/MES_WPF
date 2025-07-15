using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// ResourceView.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceView : UserControl
    {
        public ResourceView(ResourceViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}