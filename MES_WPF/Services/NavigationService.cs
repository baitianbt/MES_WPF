using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace MES_WPF.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<Type> _navigationStack = new Stack<Type>();
        private ContentControl _contentControl;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetContentControl(ContentControl contentControl)
        {
            _contentControl = contentControl;
        }

        public void NavigateTo(string viewName)
        {
            // 尝试多种命名约定来查找视图类型
            Type viewType = null;
            
            // 约定1：直接使用viewName
            viewType = Type.GetType($"MES_WPF.Views.{viewName}");
            
            // 约定2：在viewName后面添加View后缀
            if (viewType == null && !viewName.EndsWith("View"))
            {
                viewType = Type.GetType($"MES_WPF.Views.{viewName}View");
            }
            
            // 约定3：从当前程序集中查找匹配名称的类型
            if (viewType == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase) || 
                        type.Name.Equals($"{viewName}View", StringComparison.OrdinalIgnoreCase))
                    {
                        viewType = type;
                        break;
                    }
                }
            }
            
            if (viewType != null)
            {
                NavigateTo(viewType);
            }
        }

        public void NavigateTo(Type viewType)
        {
            if (_contentControl == null)
                return;

            var view = _serviceProvider.GetService(viewType);
            if (view != null)
            {
                _contentControl.Content = view;
                
                // 避免重复添加相同的视图类型
                if (_navigationStack.Count == 0 || _navigationStack.Peek() != viewType)
                {
                    _navigationStack.Push(viewType);
                }
            }
        }

        public void NavigateBack()
        {
            if (_contentControl == null || _navigationStack.Count <= 1)
                return;

            _navigationStack.Pop(); // 移除当前视图
            if (_navigationStack.Count > 0)
            {
                var previousViewType = _navigationStack.Peek();
                var view = _serviceProvider.GetService(previousViewType);
                if (view != null)
                {
                    _contentControl.Content = view;
                }
            }
        }

        public T ResolveViewModel<T>() where T : class
        {
            return _serviceProvider.GetService<T>();
        }
    }
}