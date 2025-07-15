using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// ֵת�����飬֧�ֶ��ת������˳��ִ��
    /// </summary>
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // ���û������ת������ֱ�ӷ���ԭֵ
            if (Count == 0)
            {
                return value;
            }

            // ����������ʹ�� | �ָ������������Ӧÿ��ת����
            var parameters = parameter?.ToString()?.Split('|');

            // ˳��ִ��ÿ��ת����
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