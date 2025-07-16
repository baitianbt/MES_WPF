using MES_WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            
            // 直接使用DashboardViewModel
            DataContext = new DashboardViewModel();
        }
    }
} 