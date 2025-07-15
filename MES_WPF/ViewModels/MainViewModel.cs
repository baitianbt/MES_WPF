using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MES_WPF.Core.Models;
using MES_WPF.Services;
using MES_WPF.ViewModels.SystemManagement;
using MES_WPF.Views;
using MES_WPF.Views.SystemManagement;

namespace MES_WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthenticationService _authService;
        
        private object _currentView;
        private NavigationItem _selectedNavigationItem;
        private string _currentUserName = "管理员";
        private string _statusMessage = "就绪";
        private DateTime _currentDateTime = DateTime.Now;
        private System.Timers.Timer _timer;
        private User _currentUser;
        
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        
        public NavigationItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set 
            { 
                if (SetProperty(ref _selectedNavigationItem, value) && value != null)
                {
                    NavigateToView(value.ViewType);
                }
            }
        }
        
        public string CurrentUserName
        {
            get => _currentUserName;
            set => SetProperty(ref _currentUserName, value);
        }
        
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
        
        public DateTime CurrentDateTime
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }
        
        public User CurrentUser
        {
            get => _currentUser;
            set 
            {
                if (SetProperty(ref _currentUser, value))
                {
                    CurrentUserName = value?.RealName ?? "未登录";
                }
            }
        }
        
        public ObservableCollection<NavigationItem> NavigationItems { get; } = new ObservableCollection<NavigationItem>();
        
        public ICommand LogoutCommand { get; }
        
        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IAuthenticationService authService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authService = authService;
            
            // 初始化命令
            LogoutCommand = new RelayCommand(LogoutAsync);
            
            // 初始化导航菜单
            InitializeNavigation();
            
            // 初始化时间更新
            InitializeTimer();
            
            // 获取当前用户
            CurrentUser = _authService.CurrentUser;
        }
        
        private void InitializeNavigation()
        {
            NavigationItems.Add(new NavigationItem("系统管理", "Cog", typeof(UserManagementView)));
            NavigationItems.Add(new NavigationItem("生产计划", "Calendar", null));
            NavigationItems.Add(new NavigationItem("生产执行", "Play", null));
            NavigationItems.Add(new NavigationItem("物料管理", "Package", null));
            NavigationItems.Add(new NavigationItem("质量管理", "CheckCircle", null));
            NavigationItems.Add(new NavigationItem("设备管理", "Cog", null));
            NavigationItems.Add(new NavigationItem("绩效分析", "ChartBar", null));
            
            // 默认选择第一项
            if (NavigationItems.Count > 0)
            {
                SelectedNavigationItem = NavigationItems[0];
            }
        }
        
        private void InitializeTimer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (sender, e) =>
            {
                CurrentDateTime = DateTime.Now;
            };
            _timer.Start();
        }
        
        private void NavigateToView(Type viewType)
        {
            if (viewType != null)
            {
                _navigationService.NavigateTo(viewType);
            }
        }
        
        private async void LogoutAsync()
        {
            var result = await _dialogService.ShowConfirmAsync("注销", "确定要注销当前用户吗？");
            if (result)
            {
                // 执行注销操作
                await _authService.LogoutAsync();
                
                // 显示登录界面
                var loginWindow = new Window
                {
                    Title = "登录 - MES制造执行系统",
                    Content = App.GetService<LoginView>(),
                    Width = 450,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };
                
                loginWindow.DataContext = App.GetService<LoginViewModel>();
                
                // 关闭主窗口
                Application.Current.MainWindow.Close();
                
                // 显示登录窗口
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();
            }
        }
    }
    
    public class NavigationItem
    {
        public string Title { get; }
        public string Icon { get; }
        public Type ViewType { get; }
        
        public NavigationItem(string title, string icon, Type viewType)
        {
            Title = title;
            Icon = icon;
            ViewType = viewType;
        }
    }
    
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
        
        public void Execute(object parameter)
        {
            _execute();
        }
        
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}