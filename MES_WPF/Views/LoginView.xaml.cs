using MES_WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            
            // 确保DataContext已设置后再设置默认值
            Loaded += (s, e) =>
            {
                // 设置默认测试账号（仅在调试模式下）
                #if DEBUG
                if (DataContext is LoginViewModel viewModel)
                {
                    viewModel.Username = "admin";
                    if (this.PasswordBox != null)
                    {
                        this.PasswordBox.Password = "123456";
                        // 将密码同步到ViewModel
                        viewModel.Password = this.PasswordBox.Password;
                    }
                }
                #endif
            };
        }

        /// <summary>
        /// 密码框密码变更事件处理
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel && this.PasswordBox != null)
            {
                viewModel.Password = this.PasswordBox.Password;
            }
        }
    }
}