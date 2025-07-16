using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    public class StockWarningConverter : IMultiValueConverter
    {
        // 比较 StockQuantity 与 MinimumStock：若 StockQuantity <= MinimumStock 返回 true
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is double stock &&
                values[1] is double minStock)
            {
                return stock <= minStock;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
