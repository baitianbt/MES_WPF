using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// ProductView.xaml 的交互逻辑
    /// </summary>
    public partial class ProductView : UserControl
    {
        public ProductView(ProductViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}