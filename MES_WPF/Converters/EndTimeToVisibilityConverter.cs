using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    public class EndTimeToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false; // 如果你要反转（如显示按钮时隐藏），可用

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool shouldShow = value == null ||
                              (value is DateTime dt && dt == DateTime.MinValue);

            return (Invert ? !shouldShow : shouldShow) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
