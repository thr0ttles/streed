using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Streed.Converters
{
    public class ConvertSignificantDigits : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is string && string.IsNullOrWhiteSpace(value as string)) return 0;
                var significantDigits = 2;
                if (parameter != null) significantDigits = System.Convert.ToInt32(parameter);
                var val = System.Convert.ToDouble(value);
                return Math.Round(val, significantDigits, MidpointRounding.AwayFromZero);
            }
            catch
            {
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
