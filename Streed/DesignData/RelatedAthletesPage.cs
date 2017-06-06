using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.DesignData
{
    public class RelatedAthletesPage
    {
        public List<Strava.Activities.Activity> RelatedActivities { get; set; }
        public string GroupActivityType { get; set; }

        public RelatedAthletesPage()
        {
            GroupActivityType = "Ride";
            RelatedActivities = new List<Strava.Activities.Activity>();

            RelatedActivities.Add(new Strava.Activities.Activity
                {
                    AchievementCount = 3,
                    Athlete = new Strava.Athletes.Athlete
                    {
                        FirstName = "Michael",
                        LastName = "Henasey",
                        Id = 1,
                        MediumProfileUrl = "",
                        MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                        ProfileUrl = ""
                    },
                    AverageSpeedInMetersPerSecond = 7.6420000000000003,
                    Calories = 1,
                    Description = "",
                    DistanceInMeters = 37107.199999999997,
                    ElapsedTimeInSeconds = 5108,
                    HasKudoed = true,
                    Id = 1,
                    MaxSpeedInMetersPerSecond = 11.4,
                    MovingTimeInSeconds = 4856,
                    Name = "Saw the Sun Today, Briefly",
                    TotalElevationGainInMeters = 268.0,
                    Truncated = 1,
                    TypeString = "Ride",
                    KudosCount = 21,
                    CommentCount = 0,
                    StartDateLocal = "2014-12-12T12:42:01Z",
                    Map = new Strava.Maps.Map
                        {
                            SummaryMap = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/summary_map.png", UriKind.Relative))
                        }
                });

            RelatedActivities.Add(new Strava.Activities.Activity
                {
                    AchievementCount = 0,
                    Athlete = new Strava.Athletes.Athlete
                    {
                        FirstName = "Jerry",
                        LastName = "Devlin",
                        Id = 1,
                        MediumProfileUrl = "",
                        MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                        ProfileUrl = ""
                    },
                    AverageSpeedInMetersPerSecond = 7.6420000000000003,
                    Calories = 1,
                    Description = "",
                    DistanceInMeters = 37107.199999999997,
                    ElapsedTimeInSeconds = 5108,
                    HasKudoed = false,
                    Id = 1,
                    MaxSpeedInMetersPerSecond = 11.4,
                    MovingTimeInSeconds = 4856,
                    Name = "Saw the Sun Today, Briefly and then the wind softly spoke and the sky listened but the birds still flew anyway",
                    TotalElevationGainInMeters = 268.0,
                    Truncated = 1,
                    TypeString = "Run",
                    KudosCount = 21,
                    CommentCount = 0,
                    StartDateLocal = "2014-12-12T12:42:01Z"
                });
        }
    }
}
