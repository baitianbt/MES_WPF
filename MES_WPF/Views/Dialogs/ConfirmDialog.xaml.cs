 using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views.Dialogs
{
    public partial class ConfirmDialog : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ConfirmDialog), new PropertyMetadata("确认"));
        
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(ConfirmDialog), new PropertyMetadata("您确定要执行此操作吗？"));
        
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
        
        public ConfirmDialog()
        {
            InitializeComponent();
            
            Loaded += ConfirmDialog_Loaded;
        }
        
        private void ConfirmDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTextBlock.Text = Title;
            MessageTextBlock.Text = Message;
        }
    }
}