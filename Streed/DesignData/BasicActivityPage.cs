using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Streed.DesignData
{
    public class BasicActivityPage : Strava.Activities.Activity
    {
        new public string Type { get; set; }

        public BasicActivityPage()
        {
            Type = "Ride";

            AchievementCount = 3;
            Athlete = new Strava.Athletes.Athlete
                {
                    FirstName = "Michael",
                    LastName = "Henasey",
                    Id = 1,
                    MediumProfileUrl = "",
                    MediumProfileImage = new BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                    ProfileUrl = ""
                };
            AverageSpeedInMetersPerSecond = 7.6420000000000003;
            Calories = 1000;
            Description = "";
            DistanceInMeters = 37107.199999999997;
            ElapsedTimeInSeconds = 5108;
            HasKudoed = true;
            Id = 1;
            MaxSpeedInMetersPerSecond = 11.4;
            MovingTimeInSeconds = 4856;
            Name = "Saw the Sun Today, Briefly";
            TotalElevationGainInMeters = 268.0;
            Truncated = 1;
            TypeString = "Ride";
            KudosCount = 21;
            CommentCount = 2;
            AthleteCount = 10;
            StartDate = "2014-12-12T17:42:01Z";
            StartDateLocal = "2014-12-12T12:42:01Z";
            Map = new Strava.Maps.Map
                {
                    SummaryMapWithPois = new BitmapImage(new Uri("/Assets/summary_map.png", UriKind.Relative))
                };
            WeightedAverageWatts = 229;
            AverageWatts = 204.0F;
            IsDeviceWatts = true;
            Kilojoules = 1424.0F;
            MaxHeartRate = 175;
            AverageHeartRate = 142.5F;
        }
    }
}
