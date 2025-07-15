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
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将 bool 值转换为 Visibility
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            if (value is bool b)
            {
                boolValue = b;
            }
            else if (value is int i)
            {
                boolValue = i > 0; // 关键改进点：将 Count 转为 bool
            }

            // 如果 parameter 是 "inverse"，则反转布尔值
            if (parameter != null && parameter.ToString().ToLower() == "inverse")
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 将 Visibility 转换回 bool 值
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;

                if (parameter != null && parameter.ToString().ToLower() == "inverse")
                {
                    result = !result;
                }

                return result;
            }

            return false;
        }
    }
}
