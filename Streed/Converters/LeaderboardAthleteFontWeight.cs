using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Streed.Converters
{
    public class LeaderboardAthleteFontWeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is long)
            {
                var athleteId = System.Convert.ToInt64(value);
                return (DataAccess.StreedApplicationSettings.AuthenticatedAthleteId == athleteId ? "Bold" : "Normal");
            }
            return "Normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
