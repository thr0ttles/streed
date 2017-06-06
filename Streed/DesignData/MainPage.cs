using System;
using System.Collections.Generic;

namespace Streed.DesignData
{
    public class MainPage
    {
        public string LastUpdated { get; set; }
        //public List<Strava.Activities.GroupedActivityObservableCollection> Activities { get; set; }
        public List<Strava.Activities.GroupedActivityObservableCollection> GroupedActivities { get; set; }

        public MainPage()
        {
            LastUpdated = "UPDATED 10 MINUTES AGO";
            GroupedActivities = new List<Strava.Activities.GroupedActivityObservableCollection>();
            //Activities = new List<Strava.Activities.GroupedActivityObservableCollection>();

            GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection("TODAY", DateTime.Today.ToUniversalTime(), new Strava.Activities.Activity
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
                }));

            GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection("YESTERDAY", DateTime.Today.AddDays(-1).ToUniversalTime(), new Strava.Activities.Activity
            {
                AchievementCount = 0,
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
            }));
        }
    }
}
