using CommunityToolkit.Mvvm.ComponentModel;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MES_WPF.Models
{
    /// <summary>
    /// 菜单项模型类
    /// </summary>
    public partial class MenuItemModel : ObservableObject
    {
        private string _title;
        /// <summary>
        /// 菜单标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private PackIconKind _icon;
        /// <summary>
        /// 菜单图标
        /// </summary>
        public PackIconKind Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        private bool _isExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (SetProperty(ref _isExpanded, value))
                {
                    // 更新展开/折叠图标
                    ExpandIcon = _isExpanded ? PackIconKind.ChevronUp : PackIconKind.ChevronDown;
                    
                    // 通知UI更新
                    OnPropertyChanged(nameof(SubItems));
                }
            }
        }

        private bool _isSelected;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private PackIconKind _expandIcon = PackIconKind.ChevronDown;
        /// <summary>
        /// 展开/折叠图标
        /// </summary>
        public PackIconKind ExpandIcon
        {
            get => _expandIcon;
            set => SetProperty(ref _expandIcon, value);
        }

        private string _viewName;
        /// <summary>
        /// 关联的视图名称
        /// </summary>
        public string ViewName
        {
            get => _viewName;
            set => SetProperty(ref _viewName, value);
        }

        private bool _hasPermission = true;
        /// <summary>
        /// 是否有权限访问
        /// </summary>
        public bool HasPermission
        {
            get => _hasPermission;
            set => SetProperty(ref _hasPermission, value);
        }

        /// <summary>
        /// 子菜单项集合
        /// </summary>
        public ObservableCollection<MenuItemModel> SubItems { get; } = new ObservableCollection<MenuItemModel>();

        /// <summary>
        /// 是否有子菜单
        /// </summary>
        public bool HasSubItems => SubItems.Count > 0;

        /// <summary>
        /// 菜单项点击命令
        /// </summary>
        public ICommand ExpandCommand { get; set; }
    }
} 