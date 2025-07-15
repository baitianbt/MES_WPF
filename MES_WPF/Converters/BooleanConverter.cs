using System;
using System.Globalization;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 将任何值转换为布尔值的转换器
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 参数指定的值
            if (parameter != null && parameter.ToString() != null)
            {
                // 直接比较值与参数
                return Equals(value?.ToString(), parameter.ToString());
            }

            // 空值处理
            if (value == null)
            {
                return false;
            }

            // 布尔值直接返回
            if (value is bool boolValue)
            {
                return boolValue;
            }

            // 数值类型判断非零
            if (value is int intValue)
            {
                return intValue != 0;
            }

            if (value is long longValue)
            {
                return longValue != 0;
            }

            if (value is double doubleValue)
            {
                return doubleValue != 0;
            }

            if (value is float floatValue)
            {
                return floatValue != 0;
            }

            // 字符串判断非空
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            // 集合判断非空
            if (value is System.Collections.ICollection collection)
            {
                return collection.Count > 0;
            }

            // 默认返回对象是否为null
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}