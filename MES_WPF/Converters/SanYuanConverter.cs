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
    class SanYuanConverter : IValueConverter
    { // parameter format: "1|Visible|Collapsed"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return DependencyProperty.UnsetValue;

            var parts = parameter.ToString().Split('|');
            if (parts.Length != 3) return DependencyProperty.UnsetValue;

            string matchValue = parts[0];
            string trueResult = parts[1];
            string falseResult = parts[2];

            return value.ToString() == matchValue ? trueResult : falseResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
