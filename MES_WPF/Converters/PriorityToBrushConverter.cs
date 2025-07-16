using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MES_WPF.Converters
{
    public class PriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priority)
            {
                if (priority > 7)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336")); // 红
                else if (priority >= 4)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC107")); // 黄
                else
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")); // 绿
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
