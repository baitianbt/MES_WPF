using MES_WPF.ViewModels.BasicInformation;
using System.Windows.Controls;

namespace MES_WPF.Views.BasicInformation
{
    /// <summary>
    /// BOMView.xaml 的交互逻辑
    /// </summary>
    public partial class BOMView : UserControl
    {
        public BOMView(BOMViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
} 