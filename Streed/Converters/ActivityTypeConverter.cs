using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Streed.Converters
{
    public class ActivityTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var activityType = (Strava.Activities.ActivityType)value;

            switch (activityType)
            {
                case Strava.Activities.ActivityType.Ride:
                    return char.ConvertFromUtf32(0x62);
                case Strava.Activities.ActivityType.Run:
                    return char.ConvertFromUtf32(0x80);
                case Strava.Activities.ActivityType.AlpineSki:
                case Strava.Activities.ActivityType.BackcountrySki:
                case Strava.Activities.ActivityType.CrossCountrySkiing:
                case Strava.Activities.ActivityType.NordicSki:
                case Strava.Activities.ActivityType.RollerSki:
                    return char.ConvertFromUtf32(0x87);
                case Strava.Activities.ActivityType.Swim:
                    return char.ConvertFromUtf32(0x8A);
                case Strava.Activities.ActivityType.Surfing:
                    return char.ConvertFromUtf32(0x8B);
                default:
                    return char.ConvertFromUtf32(0x86);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
