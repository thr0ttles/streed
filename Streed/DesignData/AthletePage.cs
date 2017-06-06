using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.DesignData
{
    public class AthletePage : Strava.Athletes.Athlete
    {
        public List<Strava.Segments.SegmentEffort> KOMs { get; set; }

        public AthletePage()
            : base()
        { 
            FirstName = "Michael";
            LastName = "Henasey";
            Id = 1;
            MediumProfileUrl = "";
            MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative));
            ProfileUrl = "";
            City = "Haddonfield";
            State = "NJ";
            Premium = true;
            CreatedAt = "2008-01-01T17:44:00Z";

            KOMs = new List<Strava.Segments.SegmentEffort>();
            KOMs.Add(new Strava.Segments.SegmentEffort
                {
                    Id = 1,
                    ResourceState = 3,
                    Name = "Geysers Road Climb",
                    Activity = new Strava.Activities.Activity { Id = 1 },
                    Athlete = new Strava.Athletes.Athlete { Id = 1 },
                    ElapsedTimeInSeconds = 1137,
                    MovingTimeInSeconds = 1137,
                    StartDate = "2013-03-30T18:43:50Z",
                    StartDateLocal = "2013-03-30T11:43:50Z",
                    DistanceInMeters = 6001.7F,
                    StartIndex = 5348,
                    EndIndex = 6485,
                    AverageCadence = 79.1F,
                    AverageWatts = 357.2F,
                    AverageHeartRate = 177.8F,
                    MaxHeartRate = 192,
                    Segment = new Strava.Segments.Segment
                        {
                            Id = 1,
                            ResourceState = 2,
                            Name = "Geysers Road Climb",
                            ActivityType = "Ride",
                            DistanceInMeters = 6001.74F,
                            AverageGrade = 6.2F,
                            MaxGrade = 23.5F,
                            ElevationHighInMeters = 745.6F,
                            ElevationLowInMeters = 371.0F,
                            StartLatLong = new float[2] { 38.81038486F, -122.85571538F },
                            EndLatLong = new float[2] { 38.79168709F, -122.82848568F },
                            ClimbCategory = 3,
                            City = "Cloverdale",
                            State = "California",
                            Country = "United States",
                            Private = false,
                        },
                    PRRank = 2,
                    Achievements = new Strava.Achievements.Achievement[] { new Strava.Achievements.Achievement { Rank = 3, TypeId = 3, Type = "pr" }} 
                });
            KOMs.Add(new Strava.Segments.SegmentEffort
            {
                Id = 1,
                ResourceState = 3,
                Name = "Mt.Lemmon",
                Activity = new Strava.Activities.Activity { Id = 1 },
                Athlete = new Strava.Athletes.Athlete { Id = 1 },
                ElapsedTimeInSeconds = 1137,
                MovingTimeInSeconds = 1137,
                StartDate = "2013-03-30T18:43:50Z",
                StartDateLocal = "2013-03-30T11:43:50Z",
                DistanceInMeters = 6001.7F,
                StartIndex = 5348,
                EndIndex = 6485,
                AverageCadence = 79.1F,
                AverageWatts = 357.2F,
                AverageHeartRate = 177.8F,
                MaxHeartRate = 192,
                Segment = new Strava.Segments.Segment
                {
                    Id = 1,
                    ResourceState = 2,
                    Name = "Mt.Lemmon",
                    ActivityType = "Ride",
                    DistanceInMeters = 6001.74F,
                    AverageGrade = 6.2F,
                    MaxGrade = 23.5F,
                    ElevationHighInMeters = 745.6F,
                    ElevationLowInMeters = 371.0F,
                    StartLatLong = new float[2] { 38.81038486F, -122.85571538F },
                    EndLatLong = new float[2] { 38.79168709F, -122.82848568F },
                    ClimbCategory = 3,
                    City = "Tuscon",
                    State = "Arizona",
                    Country = "United States",
                    Private = false,
                },
                PRRank = 2,
                Achievements = new Strava.Achievements.Achievement[] { new Strava.Achievements.Achievement { Rank = 7, TypeId = 2, Type = "overall" } }
            });
        }
    }
}
