using MES_WPF.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MES_WPF.Converters
{
    public class DepartmentNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<Department> departments && parameter != null)
            {
                int departmentId;
                
                // 参数可能是字符串表示的departmentId或者是绑定表达式
                if (parameter is int id)
                {
                    departmentId = id;
                }
                else if (int.TryParse(parameter.ToString(), out int parsedId))
                {
                    departmentId = parsedId;
                }
                else
                {
                    return "未知部门";
                }
                
                var department = departments.FirstOrDefault(d => d.Id == departmentId);
                return department?.DeptName ?? "未知部门";
            }
            
            return "未知部门";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 