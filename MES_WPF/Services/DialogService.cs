using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Services
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowConfirmAsync(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public Task ShowInfoAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        public Task<string> ShowInputAsync(string title, string message, string defaultValue = "")
        {
            // 创建一个简单的输入对话框
            var taskCompletionSource = new TaskCompletionSource<string>();
            
            var inputDialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                WindowStyle = WindowStyle.ToolWindow
            };
            
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) });
            
            var textBox = new TextBox { Text = defaultValue, Margin = new Thickness(0, 0, 0, 20) };
            panel.Children.Add(textBox);
            
            var buttonsPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            
            var okButton = new Button { Content = "确定", Width = 80, Margin = new Thickness(0, 0, 10, 0) };
            okButton.Click += (s, e) =>
            {
                inputDialog.DialogResult = true;
                taskCompletionSource.SetResult(textBox.Text);
                inputDialog.Close();
            };
            
            var cancelButton = new Button { Content = "取消", Width = 80 };
            cancelButton.Click += (s, e) =>
            {
                inputDialog.DialogResult = false;
                taskCompletionSource.SetResult(defaultValue);
                inputDialog.Close();
            };
            
            buttonsPanel.Children.Add(okButton);
            buttonsPanel.Children.Add(cancelButton);
            
            panel.Children.Add(buttonsPanel);
            inputDialog.Content = panel;
            
            inputDialog.ShowDialog();
            
            return taskCompletionSource.Task;
        }
    }
}