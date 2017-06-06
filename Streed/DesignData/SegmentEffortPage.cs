using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.DesignData
{
    public class SegmentEffortPage : Strava.Segments.SegmentEffort
    {
        public List<Strava.Segments.LeaderboardEffort> LeaderboardEfforts { get; set; }
        public SegmentEffortPage()
        {
            Name = "Kresson Plunge";
            Segment = new Strava.Segments.Segment
                {
                    City = "Haddonfield",
                    State = "NJ",
                    Map = new Strava.Maps.Map 
                        { 
                            SummaryMap = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/segment_map.png", UriKind.Relative)),
                            DetailMap = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/segment_map.png", UriKind.Relative))
                        },
                    ElevationHighInMeters = 100,
                    ElevationLowInMeters = 50,
                    AverageGrade = -1.0F,
                    DistanceInMeters = 6001.7F,
                };
            ElapsedTimeInSeconds = 597;
            StartDateLocal = "2014/10/31T11:01:03Z";
            Athlete = new Strava.Athletes.Athlete { FirstName = "Bill", LastName = "Anderson" };
            Activity = new Strava.Activities.Activity { Name = "NSR" };

            LeaderboardEfforts = new List<Strava.Segments.LeaderboardEffort>();
            LeaderboardEfforts.Add(new Strava.Segments.LeaderboardEffort
            {
                Rank = 1,
                AthleteName = "Michael Henasey",
                AthleteId = 0,
                MovingTimeInSeconds = 1137,
                ElapsedTimeInSeconds = 397,
                DistanceInMeters = 6001.7F,
                SegmentDistanceInMeters = 6001.7F,
            });
            LeaderboardEfforts.Add(new Strava.Segments.LeaderboardEffort
            {
                Rank = 2,
                AthleteName = "Johhny 'Cash'",
                AthleteId = 1,
                MovingTimeInSeconds = 1157,
                ElapsedTimeInSeconds = 1157,
                DistanceInMeters = 6001.7F,
                SegmentDistanceInMeters = 6001.7F,
            });
            LeaderboardEfforts.Add(new Strava.Segments.LeaderboardEffort
            {
                Rank = 105,
                AthleteName = "Bill Anderson",
                AthleteId = 2,
                MovingTimeInSeconds = 1157,
                ElapsedTimeInSeconds = 1157,
                DistanceInMeters = 6001.7F,
                SegmentDistanceInMeters = 6001.7F,
            });
            LeaderboardEfforts.Add(new Strava.Segments.LeaderboardEffort
            {
                Rank = 9999,
                AthleteName = "Robert Palmer",
                AthleteId = 3,
                MovingTimeInSeconds = 1157,
                ElapsedTimeInSeconds = 11157,
                DistanceInMeters = 6001.7F,
                SegmentDistanceInMeters = 6001.7F,
            });
        }
    }
}
