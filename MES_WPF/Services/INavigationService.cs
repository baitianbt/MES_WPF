using System;
using System.Windows.Controls;

namespace MES_WPF.Services
{
    public interface INavigationService
    {
        void NavigateTo(string viewName);
        void NavigateTo(Type viewType);
        void NavigateBack();
        T ResolveViewModel<T>() where T : class;
        void SetContentControl(ContentControl contentControl);
    }
}