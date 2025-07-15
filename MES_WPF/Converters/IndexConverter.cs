using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    public class IndexConverter : IValueConverter
    {
        // value: Status(1/2/3), parameter = -1
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;

            if (int.TryParse(value.ToString(), out int status))
            {
                int offset = System.Convert.ToInt32(parameter);
                return status + offset;
            }

            return -1;
        }

        // value: SelectedIndex(0/1/2), parameter = -1
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            if (int.TryParse(value.ToString(), out int index))
            {
                int offset = System.Convert.ToInt32(parameter);
                return index - offset;
            }

            return 0;
        }
    }

}
