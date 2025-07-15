 using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views.Dialogs
{
    public partial class InputDialog : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(InputDialog), new PropertyMetadata("输入"));
        
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(InputDialog), new PropertyMetadata("请输入："));
        
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register(nameof(Input), typeof(string), typeof(InputDialog), new PropertyMetadata(string.Empty));
        
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        
        public string Input
        {
            get => (string)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
        
        public InputDialog()
        {
            InitializeComponent();
            
            Loaded += InputDialog_Loaded;
        }
        
        private void InputDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTextBlock.Text = Title;
            MessageTextBlock.Text = Message;
            InputTextBox.Text = Input;
            
            // 自动聚焦输入框
            InputTextBox.Focus();
        }
        
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // 关闭对话框并返回输入值
            DialogHost.CloseDialogCommand.Execute(InputTextBox.Text, null);
        }
    }
}