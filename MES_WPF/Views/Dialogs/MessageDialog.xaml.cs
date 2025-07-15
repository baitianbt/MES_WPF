 using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MES_WPF.Views.Dialogs
{
    public partial class MessageDialog : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MessageDialog), new PropertyMetadata("消息"));
        
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageDialog), new PropertyMetadata("操作已完成。"));
        
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(PackIconKind), typeof(MessageDialog), new PropertyMetadata(PackIconKind.Information));
        
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
        
        public PackIconKind Icon
        {
            get => (PackIconKind)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        
        public MessageDialog()
        {
            InitializeComponent();
            
            Loaded += MessageDialog_Loaded;
        }
        
        private void MessageDialog_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTextBlock.Text = Title;
            MessageTextBlock.Text = Message;
            IconControl.Kind = Icon;
        }
    }
}