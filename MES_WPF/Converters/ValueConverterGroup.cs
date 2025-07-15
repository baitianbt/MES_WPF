using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 值转换器组，支持多个转换器的顺序执行
    /// </summary>
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 如果没有配置转换器，直接返回原值
            if (Count == 0)
            {
                return value;
            }

            // 解析参数：使用 | 分隔多个参数，对应每个转换器
            var parameters = parameter?.ToString()?.Split('|');

            // 顺序执行每个转换器
            for (int i = 0; i < Count; i++)
            {
                var currentParam = parameters != null && i < parameters.Length ? parameters[i] : null;
                value = this[i].Convert(value, targetType, currentParam, culture);
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ValueConverterGroup does not support ConvertBack");
        }
    }
}