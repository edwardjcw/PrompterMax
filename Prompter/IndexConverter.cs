using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Prompter
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int)value;
            switch (targetType.Name)
            {
                case "String":
                    return $"{index + 1}";
                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (string)value;
            switch (targetType.Name)
            {
                case "Int32":
                    return $"{int.Parse(index) - 1}"; // where I stopped
                default:
                    return value;
            }
        }
    }
}
