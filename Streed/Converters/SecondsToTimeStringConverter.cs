using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Streed.Converters
{
    public class SecondsToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = System.Convert.ToInt32(value);

            //if (value is string)
            //{
            //    int seconds;
            //    if (false == int.TryParse((string)value, out seconds))
            //        return string.Empty;
            //    value = seconds;
            //}
            
            //var dt = new DateTime();
            //if (value is int)
            //    dt.AddSeconds((int)value);
            //else
            //    dt.AddSeconds((double)value);

            var dt = new DateTime().AddSeconds(val);
            if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
            if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
            return string.Format(":{0:ss}", dt);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
