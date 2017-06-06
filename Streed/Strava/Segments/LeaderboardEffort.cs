using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Segments
{
    [DataContract]
    public class LeaderboardEffort
    {
        [DataMember(Name = "athlete_name")]
        public string AthleteName { get; set; }

        [DataMember(Name = "athlete_id")]
        public long AthleteId { get; set; }

        [DataMember(Name = "distance")]
        public float DistanceInMeters { get; set; }

        public float SegmentDistanceInMeters { get; set; }

        [DataMember(Name = "elapsed_time")]
        public int ElapsedTimeInSeconds { get; set; }

        [DataMember(Name = "moving_Time")]
        public int MovingTimeInSeconds { get; set; }

        [DataMember(Name = "rank")]
        public int Rank { get; set; }

        [DataMember(Name = "activity_id")]
        public long ActivityId { get; set; }

        [DataMember(Name = "effort_id")]
        public long EffortId { get; set; }

        public string Speed
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    var distanceInKilometers = SegmentDistanceInMeters / 1000;
                    distanceInKilometers = (float)Math.Round(distanceInKilometers, 1, MidpointRounding.AwayFromZero);
                    var speed = (distanceInKilometers / ElapsedTimeInSeconds) * 60 * 60;
                    return string.Format("{0:0.0}", speed);
                }
                else
                {
                    var distanceInMiles = SegmentDistanceInMeters * 0.00062137119;
                    distanceInMiles = Math.Round(distanceInMiles, 1, MidpointRounding.AwayFromZero);
                    var speed = (distanceInMiles / ElapsedTimeInSeconds) * 60 * 60;
                    return string.Format("{0:0.0}", speed);
                }
            }
        }

        public string SpeedUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                    return "kph";
                else
                    return "mph";
            }
        }

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
                if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
                if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
                return string.Format(":{0:ss}", dt);
            }
        }

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
    }
}
