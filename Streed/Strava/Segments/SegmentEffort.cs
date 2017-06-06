using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Segments
{
    [DataContract]
    public class SegmentEffort
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "resource_state")]
        public int ResourceState { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "activity")]
        public Strava.Activities.Activity Activity { get; set; }

        [DataMember(Name = "athlete")]
        public Strava.Athletes.Athlete Athlete { get; set; }

        [DataMember(Name = "elapsed_time")]
        public int ElapsedTimeInSeconds { get; set; }

        public string ElapsedTime 
        {
            get
            {
                var dt = new DateTime().AddSeconds(ElapsedTimeInSeconds);
                if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
                if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
                return string.Format(":{0:ss}", dt);
            }
        }

        [DataMember(Name = "moving_time")]
        public int MovingTimeInSeconds { get; set; }

        [DataMember(Name = "start_date")]
        public string StartDate { get; set; }

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

        [DataMember(Name = "distance")]
        public float DistanceInMeters { get; set; }

        [DataMember(Name = "start_index")]
        public int StartIndex { get; set; }

        [DataMember(Name = "end_index")]
        public int EndIndex { get; set; }

        [DataMember(Name = "average_cadence")]
        public float AverageCadence { get; set; }

        [DataMember(Name = "average_watts")]
        public float AverageWatts { get; set; }

        [DataMember(Name = "average_heartrate")]
        public float AverageHeartRate { get; set; }

        [DataMember(Name = "max_heartrate")]
        public float MaxHeartRate { get; set; }

        [DataMember(Name = "segment")]
        public Segment Segment { get; set; }

        [DataMember(Name = "kom_rank")]
        public int? KomRank { get; set; }

        [DataMember(Name = "pr_rank")]
        public int? PRRank { get; set; }

        [DataMember(Name = "hidden")]
        public bool Hidden { get; set; }

        [DataMember(Name = "achievements")]
        public Strava.Achievements.Achievement[] Achievements { get; set; }

        public string Distance
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    var distanceInKilometers = DistanceInMeters / 1000;
                    distanceInKilometers = (float)Math.Round(distanceInKilometers, 1, MidpointRounding.AwayFromZero);
                    if (distanceInKilometers - (int)distanceInKilometers == 0)
                        return string.Format("{0:0}", distanceInKilometers);
                    else
                        return string.Format("{0:0.0}", distanceInKilometers);
                }
                else
                {
                    var distanceInMiles = DistanceInMeters * 0.00062137119;
                    distanceInMiles = Math.Round(distanceInMiles, 1, MidpointRounding.AwayFromZero);
                    if (distanceInMiles - (int)distanceInMiles == 0)
                        return string.Format("{0:0}", distanceInMiles);
                    else
                        return string.Format("{0:0.0}", distanceInMiles);
                }
            }
        }

        public string DistanceUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    return "km";
                }
                else
                {
                    return "mi";
                }
            }
        }

        public string MovingTime
        {
            get
            {
                var dt = new DateTime().AddSeconds(MovingTimeInSeconds);
                if (dt.Hour > 0)
                    return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
                else if (dt.Minute > 0)
                    return string.Format("{0:mm}:{1:ss}", dt, dt);
                else
                    return string.Format(":{0:ss}", dt);
            }
        }

        public string Stats
        {
            get
            {
                return string.Format("{0} {1}, {2}% Avg grade, {3}", Distance, DistanceUnit, Segment.AverageGrade, MovingTime);
            }
        }

        public bool IsAchievementKom
        {
            get
            {
                return Achievements.Where(w => w.TypeId == 2 || w.TypeId == 5).Any();
            }
        }

        public int AchievementTypeId 
        {
            get
            {
                if (Achievements.Where(w => w.TypeId == 2).Any()) return 2;
                if (Achievements.Where(w => w.TypeId == 5).Any()) return 5;
                return 3;
            }
        }

        public string AchievementRank
        {
            get
            {
                if (Achievements.Where(w => w.TypeId == 2 && w.Rank == 1).Any()) return string.Empty;
                if (Achievements.Where(w => w.TypeId == 2).Any()) return Achievements.Where(w => w.TypeId == 2).Take(1).Single().Rank.ToString();
                if (Achievements.Where(w => w.TypeId == 5 && w.Rank == 1).Any()) return string.Empty;
                if (Achievements.Where(w => w.TypeId == 5).Any()) return Achievements.Where(w => w.TypeId == 5).Take(1).Single().Rank.ToString();
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 1).Any()) return "1";
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 2).Any()) return "2";
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 3).Any()) return "3";

                return string.Empty;
            }
        }

        public Uri AchievementIconUri
        {
            get
            {
                if (Achievements.Where(w => w.TypeId == 2 && w.Rank == 1).Any()) return new Uri("/Assets/kom-overall.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 2).Any()) return new Uri("/Assets/trophy-overall.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 5 && w.Rank == 1).Any()) return new Uri("/Assets/kom-year.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 5).Any()) return new Uri("/Assets/trophy-year.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 1).Any()) return new Uri("/Assets/pr-1st.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 2).Any()) return new Uri("/Assets/pr-2nd.png", UriKind.Relative);
                if (Achievements.Where(w => w.TypeId == 3 && w.Rank == 3).Any()) return new Uri("/Assets/pr-3rd.png", UriKind.Relative);

                return new Uri("/Assets/kom-icon-transparent.png", UriKind.Relative);
            }
        }

        public string OverallBestEffort { get; set; }
        public string YearlyBestEffort { get; set; }
        public string OverallRank { get; set; }
        public string YearlyRank { get; set; }
    }
}
