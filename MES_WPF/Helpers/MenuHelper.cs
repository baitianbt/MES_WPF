using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace MES_WPF.Helpers
{
    /// <summary>
    /// 菜单帮助类，用于处理菜单的样式和交互
    /// </summary>
    public static class MenuHelper
    {
        /// <summary>
        /// 设置菜单项的展开/折叠事件处理
        /// </summary>
        /// <param name="menuItem">菜单项</param>
        public static void SetupMenuItem(TreeViewItem menuItem)
        {
            if (menuItem == null) return;
            
            // 添加展开/折叠事件处理
            menuItem.Expanded += MenuItemExpanded;
            menuItem.Collapsed += MenuItemCollapsed;
            
            // 为菜单头部添加点击事件，用于手动切换展开状态
            if (menuItem.Header is Grid headerGrid)
            {
                headerGrid.MouseLeftButtonDown += (s, args) => 
                {
                    menuItem.IsExpanded = !menuItem.IsExpanded;
                    args.Handled = true; // 防止事件冒泡
                };
            }
        }
        
        /// <summary>
        /// 菜单项展开事件处理
        /// </summary>
        private static void MenuItemExpanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item && item.Header is Grid grid)
            {
                // 查找ChevronDown图标并旋转为ChevronUp
                var packIcon = FindVisualChild<PackIcon>(grid);
                if (packIcon != null && packIcon.Kind == PackIconKind.ChevronDown)
                {
                    packIcon.Kind = PackIconKind.ChevronUp;
                }
            }
        }
        
        /// <summary>
        /// 菜单项折叠事件处理
        /// </summary>
        private static void MenuItemCollapsed(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem item && item.Header is Grid grid)
            {
                // 查找ChevronUp图标并旋转为ChevronDown
                var packIcon = FindVisualChild<PackIcon>(grid);
                if (packIcon != null && packIcon.Kind == PackIconKind.ChevronUp)
                {
                    packIcon.Kind = PackIconKind.ChevronDown;
                }
            }
        }
        
        /// <summary>
        /// 查找视觉树中的指定类型子元素
        /// </summary>
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T typedChild)
                {
                    return typedChild;
                }
                
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 查找TreeViewItem的父级TreeViewItem
        /// </summary>
        public static TreeViewItem FindParentTreeViewItem(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeViewItem))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as TreeViewItem;
        }
        
        /// <summary>
        /// 获取菜单项的标题文本
        /// </summary>
        public static string GetMenuItemTitle(TreeViewItem item)
        {
            if (item == null) return string.Empty;
            
            var header = item.Header;
            
            if (header is Grid grid)
            {
                // 查找StackPanel中的TextBlock
                var stackPanel = FindVisualChild<StackPanel>(grid);
                if (stackPanel != null)
                {
                    var textBlock = FindVisualChild<TextBlock>(stackPanel);
                    return textBlock?.Text ?? string.Empty;
                }
            }
            else if (header is string headerText)
            {
                return headerText;
            }
            
            return string.Empty;
        }
    }
} 