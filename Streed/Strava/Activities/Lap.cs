using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Activities
{
    [DataContract]
    public class Lap
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "resource_state")]
        public int ResourceState { get; set; }

        [DataMember(Name = "activity")]
        public Activity Activity { get; set; }

        [DataMember(Name = "athlete")]
        public Athletes.Athlete Athlete { get; set; }

        [DataMember(Name = "elapsed_time")]
        public int ElapsedTimeInSeconds { get; set; }

        public string ElapsedTime { get { return Utilities.TimeFromTimeInSeconds(ElapsedTimeInSeconds); } }

        [DataMember(Name = "moving_time")]
        public int MovingTimeInSeconds { get; set; }

        public string MovingTime { get { return Utilities.TimeFromTimeInSeconds(MovingTimeInSeconds); } }

        [DataMember(Name = "start_date")]
        public string StartDate { get; set; }

        [DataMember(Name = "start_date_local")]
        public string StartDateLocal { get; set; }

        [DataMember(Name = "distance")]
        public double DistanceInMeters { get; set; }

        public string Distance { get { return Utilities.DistanceFromDistanceInMeters(DistanceInMeters); } }
        public string DistanceUnit { get { return Utilities.DistanceUnit; } }

        [DataMember(Name = "start_index")]
        public int StartIndex { get; set; }

        [DataMember(Name = "end_index")]
        public int EndIndex { get; set; }

        [DataMember(Name = "total_elevation_gain")]
        public double TotalElevationGainInMeters { get; set; }

        public string TotalElevationGain { get { return Utilities.TotalElevationGainFromElevationGainInMeters(TotalElevationGainInMeters); } }
        public string TotalElevationGainUnit { get { return Utilities.TotalElevationGainUnit; } }

        [DataMember(Name = "average_speed")]
        public double AverageSpeedInMetersPerSecond { get; set; }

        public string AverageSpeed { get { return Utilities.SpeedFromSpeedInMetersPerSecond(AverageSpeedInMetersPerSecond); } }
        public string AverageSpeedUnit { get { return Utilities.SpeedUnit; } }

        [DataMember(Name = "max_speed")]
        public double MaxSpeedInMetersPerSecond { get; set; }

        public string MaxSpeed { get { return Utilities.SpeedFromSpeedInMetersPerSecond(MaxSpeedInMetersPerSecond); } }
        public string MaxSpeedUnit { get { return Utilities.SpeedUnit; } }

        [DataMember(Name = "average_cadence")]
        public double AverageCadence { get; set; }

        [DataMember(Name = "average_watts")]
        public double AverageWatts { get; set; }

        public int AvgWatts { get { return Convert.ToInt32(AverageWatts); } }

        [DataMember(Name = "device_watts")]
        public bool DeviceWatts { get; set; }

        [DataMember(Name = "average_heartrate")]
        public double AverageHeartRate { get; set; }

        [DataMember(Name = "max_heartrate")]
        public double MaxHeartRate { get; set; }

        [DataMember(Name = "lap_index")]
        public int LapIndex { get; set; }

        [DataMember]
        public double TSS { get; set; }

        [DataMember]
        public double IF { get; set; }

        [DataMember]
        public double VI { get; set; }

        [DataMember]
        public double EF { get; set; }

        [DataMember]
        public double PwHr { get; set; }

        [DataMember]
        public double WKg { get; set; }

        [DataMember]
        public double WeightedAverageWatts { get; set; }
    }
}
