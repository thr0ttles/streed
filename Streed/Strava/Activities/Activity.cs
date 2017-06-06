using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace Streed.Strava.Activities
{
    [DataContract]
    public class Activity : INotifyPropertyChanged
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "resource_state")]
        public int ResourceState { get; set; } 

        [DataMember(Name = "athlete")]
        public Athletes.Athlete Athlete { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "distance")]
        public double DistanceInMeters { get; set; }

        public string Distance 
        { 
            get 
            {
                return Utilities.DistanceFromDistanceInMeters(DistanceInMeters);
            } 
        }

        public string DistanceUnit 
        { 
            get 
            {
                return Utilities.DistanceUnit;
            } 
        }

        [DataMember(Name = "moving_time")]
        public int MovingTimeInSeconds { get; set; }

        public string MovingTime 
            { 
                get 
                {
                    var dt = new DateTime().AddSeconds(MovingTimeInSeconds);
                    if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
                    if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
                    return string.Format(":{0:ss}", dt);
                } 
        }

        [DataMember(Name = "elapsed_time")]
        public int ElapsedTimeInSeconds { get; set; }

        public string ElapsedTime 
        { 
            get 
            {
                return Utilities.TimeFromTimeInSeconds(ElapsedTimeInSeconds);
            } 
        }

        [DataMember(Name = "total_elevation_gain")]
        public double TotalElevationGainInMeters { get; set; }

        public string TotalElevationGain 
        { 
            get 
            {
                return Utilities.TotalElevationGainFromElevationGainInMeters(TotalElevationGainInMeters);
            } 
        }

        public string TotalElevationGainUnit
        {
            get
            {
                return Utilities.TotalElevationGainUnit;
            }
        }

        public string TotalElevationGainOrPace
        {
            get
            {
                switch (Type)
                {
                    case Strava.Activities.ActivityType.Ride:
                        return TotalElevationGain;
                    case Strava.Activities.ActivityType.Run:
                        return Pace;
                    case Strava.Activities.ActivityType.AlpineSki:
                    case Strava.Activities.ActivityType.BackcountrySki:
                    case Strava.Activities.ActivityType.CrossCountrySkiing:
                    case Strava.Activities.ActivityType.NordicSki:
                    case Strava.Activities.ActivityType.RollerSki:
                        return TotalElevationGain;
                    case Strava.Activities.ActivityType.Swim:
                        return Pace;
                    case Strava.Activities.ActivityType.Surfing:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }

        public string TotalElevationGainOrPaceUnit
        {
            get
            {
                switch (Type)
                {
                    case Strava.Activities.ActivityType.Ride:
                        return TotalElevationGainUnit;
                    case Strava.Activities.ActivityType.Run:
                        return PaceUnit;
                    case Strava.Activities.ActivityType.AlpineSki:
                    case Strava.Activities.ActivityType.BackcountrySki:
                    case Strava.Activities.ActivityType.CrossCountrySkiing:
                    case Strava.Activities.ActivityType.NordicSki:
                    case Strava.Activities.ActivityType.RollerSki:
                        return TotalElevationGainUnit;
                    case Strava.Activities.ActivityType.Swim:
                        return PaceUnit;
                    case Strava.Activities.ActivityType.Surfing:
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }

        [DataMember(Name = "type")]
        public string TypeString { get; set; }

        public ActivityType Type 
        {
            get
            {
                ActivityType activityType;
                if (Enum.TryParse<ActivityType>(TypeString, out activityType))
                    return activityType;
                else
                    return ActivityType.Workout;
            }
        }

        [DataMember(Name = "average_speed")]
        public double AverageSpeedInMetersPerSecond { get; set; }

        public string AverageSpeed 
        { 
            get 
            {
                return Utilities.SpeedFromSpeedInMetersPerSecond(AverageSpeedInMetersPerSecond);
            } 
        }

        [DataMember(Name = "max_speed")]
        public double MaxSpeedInMetersPerSecond { get; set; }

        public string MaxSpeed 
        { 
            get
            {
                return Utilities.SpeedFromSpeedInMetersPerSecond(MaxSpeedInMetersPerSecond);
            } 
        }

        public string Pace
        {
            get
            {
                switch (Type)
                {
                    case ActivityType.Swim:
                    case ActivityType.Run:
                        {
                            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                            {
                                var paceInSeconds = 1 / (AverageSpeedInMetersPerSecond / 1000);
                                paceInSeconds = Math.Round(paceInSeconds, 0, MidpointRounding.AwayFromZero);
                                var dt = new TimeSpan(0, 0, (int)paceInSeconds);
                                return string.Format("{0:%m}:{1:ss}", dt, dt);
                            }
                            else
                            {
                                var paceInSeconds = 1 / (AverageSpeedInMetersPerSecond * 0.00062137119);
                                paceInSeconds = Math.Round(paceInSeconds, 0, MidpointRounding.AwayFromZero);
                                var dt = new TimeSpan(0, 0, (int)paceInSeconds);
                                return string.Format("{0:%m}:{1:ss}", dt, dt);
                            }
                        }
                    case ActivityType.Ride:
                        return string.Format("{0}", AverageSpeed);
                    default:
                        return string.Empty;
                }
            }
        }

        public string PaceUnit
        {
            get
            {
                switch (Type)
                {
                    case ActivityType.Swim:
                    case ActivityType.Run:
                        {
                            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                                return "/km";
                            else
                                return "/mi";
                        }
                    case ActivityType.Ride:
                        {
                            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                                return "kph";
                            else
                                return "mph";
                        }
                    default:
                        return string.Empty;
                }
            }
        }

        public string PaceLabel
        {
            get
            {
                switch (Type)
                {
                    case ActivityType.Swim:
                    case ActivityType.Run:
                        return "Avg Pace";
                    case ActivityType.Ride:
                        return "Avg Speed";
                    default:
                        return string.Empty;
                }
            }
        }

        [DataMember(Name = "calories")]
        public float Calories { get; set; }

        public string CaloriesString
            { 
                get 
                { 
                    var calories = Math.Round(Calories, 0, MidpointRounding.AwayFromZero);
                    return string.Format("{0:#}", calories);
                } 
            }

        [DataMember(Name = "truncated")]
        public int Truncated { get; set; }

        [DataMember(Name = "has_kudoed")]
        public bool HasKudoed { get; set; }

        public bool CanKudos 
        { 
            get 
            { 
                if (HasKudoed) return false; 
                return (DataAccess.StreedApplicationSettings.AuthenticatedAthleteId != Athlete.Id); 
            } 
        }

        [DataMember(Name = "achievement_count")]
        public int AchievementCount { get; set; }

        [DataMember(Name = "kudos_count")]
        public int KudosCount { get; set; }

        [DataMember(Name = "comment_count")]
        public int CommentCount { get; set; }

        [DataMember(Name = "athlete_count")]
        public int AthleteCount { get; set; }

        public int CorrectedAthleteCount { get { return AthleteCount - 1; } }

        [DataMember(Name="map")]
        public Maps.Map Map { get; set; }

        [DataMember(Name = "start_date")]
        public string StartDate { get; set; }

        public DateTime StartDateTime 
        {
            get
            {
                DateTime dt;
                DateTime.TryParse(StartDate, out dt);
                var utc = dt.ToUniversalTime();
                return utc;
            }
        }

        [DataMember(Name = "start_date_local")]
        public string StartDateLocal { get; set; }

        public DateTime StartDateTimeLocal
        {
            get
            {
                DateTime dt;
                DateTime.TryParse(StartDateLocal, out dt);
                var utc = dt.ToUniversalTime();
                return utc;
            }
        }

        public string StartTimeLocal
        {
            get
            {
                DateTime dt;
                DateTime.TryParse(StartDateLocal, out dt);
                var utc = dt.ToUniversalTime().ToShortTimeString();
                return utc;
            }
        }

        [DataMember(Name = "timezone")]
        public string StartDateTimezone { get; set; }

        public string StartDateTimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - StartDateTime;
                if ((int)diff.TotalDays > 730)
                {
                    return string.Format("{0} years ago", (int)(diff.TotalDays / 365));
                }
                else if ((int)diff.TotalDays > 365)
                {
                    return "over a year ago";
                }
                else if ((int)diff.TotalDays > 30)
                {
                    return string.Format("{0} months ago", (int)(diff.TotalDays / 30));
                }
                else if ((int)diff.TotalDays > 1)
                {
                    return string.Format("{0} days ago", (int)diff.TotalDays);
                }
                else if ((int)diff.TotalDays == 1)
                {
                    return "1 day ago";
                }
                else if ((int)diff.TotalHours > 1)
                {
                    return string.Format("{0} hours ago", (int)diff.TotalHours);
                }
                else if ((int)diff.TotalHours == 1)
                {
                    return "1 hour ago";
                }
                else if ((int)diff.TotalMinutes > 1)
                {
                    return string.Format("{0} minutes ago", (int)diff.TotalMinutes);
                }
                else if ((int)diff.TotalMinutes == 1)
                {
                    return "over a minute ago";
                }
                else if ((int)diff.TotalSeconds == 0)
                {
                    return "a moment ago";
                }
                else
                {
                    if ((int)diff.TotalSeconds < 0)
                        return string.Empty;

                    return string.Format("{0} seconds ago", (int)diff.TotalSeconds);
                }
            }
        }

        [DataMember(Name = "private")]
        public bool Private { get; set; }

        [DataMember(Name = "segment_efforts")]
        public Strava.Segments.SegmentEffort[] SegmentEfforts { get; set; }

        [DataMember(Name = "average_watts")]
        public float AverageWatts { get; set; }
        
        [DataMember(Name = "weighted_average_watts")]
        public int WeightedAverageWatts { get; set; }

        public int AvgWatts
        {
            get
            {
                if (WeightedAverageWatts != 0) return WeightedAverageWatts;
                return Convert.ToInt32(AverageWatts);
            }
        }

        [DataMember(Name = "kilojoules")]
        public float Kilojoules { get; set; }

        [DataMember(Name = "device_watts")]
        public bool IsDeviceWatts { get; set; }

        [DataMember(Name = "average_heartrate")]
        public float AverageHeartRate { get; set; }

        public int AverageHeartRateInt { get { return Convert.ToInt32(AverageHeartRate); } }

        [DataMember(Name = "max_heartrate")]
        public float MaxHeartRate { get; set; }

        [DataMember(Name = "average_temp")]
        public double AverageTempInCelsius { get; set; }

        public double AverageTemp { get { return Utilities.TempFromTempInCelcius(AverageTempInCelsius); } }
        public int AverageTempInt { get { return Convert.ToInt32(Utilities.TempFromTempInCelcius(AverageTempInCelsius)); } }
        public string AverageTempUnit { get { return Utilities.TempUnit; } }

        [DataMember(Name = "average_cadence")]
        public double AverageCadence { get; set; }

        [DataMember]
        public ActivityZone[] ActivityZones { get; set; }

        [DataMember]
        public Lap[] Laps { get; set; }

        [DataMember]
        public Streams.Stream WattsStream { get; set; }

        [DataMember]
        public Streams.Stream HeartRateStream { get; set; }

        [DataMember]
        public Streams.Stream DistanceStream { get; set; }

        [DataMember]
        public Streams.Stream AltitudeStream { get; set; }

        [DataMember]
        public Streams.Stream TimeStream { get; set; }

        [DataMember]
        public Streams.Stream GradientStream { get; set; }

        [DataMember]
        public bool WasLoadedByFeedPage { get; set; }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}