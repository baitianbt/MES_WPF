using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 将字符串转换为可见性的转换器
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将字符串转换为可见性
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="parameter">转换参数</param>
        /// <param name="culture">区域性信息</param>
        /// <returns>如果字符串不为空，则返回Visible，否则返回Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            }
            
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 将可见性转换为字符串（不支持）
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 