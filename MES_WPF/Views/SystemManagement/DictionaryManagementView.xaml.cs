using MES_WPF.ViewModels.SystemManagement;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MES_WPF.Views.SystemManagement
{
    /// <summary>
    /// DictionaryManagementView.xaml 的交互逻辑
    /// </summary>
    public partial class DictionaryManagementView : UserControl
    {
        public DictionaryManagementView(DictionaryManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // 加载完成后刷新数据
            Loaded += (s, e) =>
            {
                if (DataContext is DictionaryManagementViewModel vm)
                {
                    _ = vm.RefreshDictionariesCommand.ExecuteAsync(null);
                }
            };
        }

    }
} 