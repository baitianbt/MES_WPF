using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            
            // 可以通过代码设置DataContext，也可以通过XAML绑定
            // DataContext = new DashboardViewModel();
            
            // 保持原来的数据绑定，使用MainWindow的DataContext
            if (Application.Current.MainWindow != null)
            {
                DataContext = Application.Current.MainWindow.DataContext;
            }
        }
    }
} 