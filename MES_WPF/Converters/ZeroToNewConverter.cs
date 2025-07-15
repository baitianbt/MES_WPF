using System;
using System.Globalization;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 将ID为0的实体显示为"新建xxx"，否则显示"编辑xxx"
    /// </summary>
    public class ZeroToNewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id)
            {
                string entityName = parameter as string ?? "数据";
                return id == 0 ? $"新建{entityName}" : $"编辑{entityName}";
            }
            return "编辑";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 