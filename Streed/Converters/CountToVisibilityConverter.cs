using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Streed.Converters
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var reverse = false;
            if (parameter != null && parameter is bool)
            {
                reverse = (bool)parameter;
            }

            if (value == null)
            {
                return Visibility.Collapsed;
            }

            var val = System.Convert.ToInt32(value);
            if (reverse) return (val > 0 ? Visibility.Collapsed : Visibility.Visible);
            else return (val > 0 ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class MoreThanOneToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            else if (value is string)
            {
                int val;
                int.TryParse(value as string, out val);
                return (val > 1 ? Visibility.Visible : Visibility.Collapsed);
            }
            else if (value is int || value is float)
            {
                return ((int)value > 1 ? Visibility.Visible : Visibility.Collapsed);
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
