using System;
using System.Globalization;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 设备维护需求转换器，判断下次维护日期是否小于等于当前日期
    /// </summary>
    public class MaintenanceRequiredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime nextMaintenanceDate)
            {
                return nextMaintenanceDate.Date <= DateTime.Today;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 