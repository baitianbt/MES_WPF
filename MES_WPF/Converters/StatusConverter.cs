using System;
using System.Globalization;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    /// <summary>
    /// 状态转换器，将数字状态转换为文本描述
    /// </summary>
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte status)
            {
                return status switch
                {
                    1 => "草稿",
                    2 => "审核中",
                    3 => "已发布",
                    4 => "已作废",
                    _ => "未知"
                };
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string statusText)
            {
                return statusText switch
                {
                    "草稿" => (byte)1,
                    "审核中" => (byte)2,
                    "已发布" => (byte)3,
                    "已作废" => (byte)4,
                    _ => (byte)0
                };
            }
            return (byte)0;
        }
    }
}