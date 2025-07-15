using System;
using System.Globalization;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// ���κ�ֵת��Ϊ����ֵ��ת����
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // ����ָ����ֵ
            if (parameter != null && parameter.ToString() != null)
            {
                // ֱ�ӱȽ�ֵ�����
                return Equals(value?.ToString(), parameter.ToString());
            }

            // ��ֵ����
            if (value == null)
            {
                return false;
            }

            // ����ֱֵ�ӷ���
            if (value is bool boolValue)
            {
                return boolValue;
            }

            // ��ֵ�����жϷ���
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

            // �ַ����жϷǿ�
            if (value is string stringValue)
            {
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            // �����жϷǿ�
            if (value is System.Collections.ICollection collection)
            {
                return collection.Count > 0;
            }

            // Ĭ�Ϸ��ض����Ƿ�Ϊnull
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}