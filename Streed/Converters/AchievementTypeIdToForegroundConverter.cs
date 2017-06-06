using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Streed.Converters
{
    public class AchievementTypeIdToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                var typeid = (int)value;
                if (typeid == 2) return App.Current.Resources["WhiteBrush"];
                if (typeid == 5) return App.Current.Resources["TrophyBrush"];
                return App.Current.Resources["WhiteBrush"];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
