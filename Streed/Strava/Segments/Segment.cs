using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Segments
{
    [DataContract]
    public class Segment
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "resource_state")]
        public int ResourceState { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "activity_type")]
        public string ActivityType { get; set; }

        [DataMember(Name = "distance")]
        public float DistanceInMeters { get; set; }

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

        [DataMember(Name = "average_grade")]
        public float AverageGrade { get; set; }

        [DataMember(Name = "maximum_grade")]
        public float MaxGrade { get; set; }

        [DataMember(Name = "elevation_high")]
        public float ElevationHighInMeters { get; set; }

        [DataMember(Name = "elevation_low")]
        public float ElevationLowInMeters { get; set; }

        public string ElevationDifference 
        { 
            get 
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    var elevationDiff = ElevationLowInMeters - ElevationHighInMeters;
                    elevationDiff = (float)Math.Round(elevationDiff, 0, MidpointRounding.AwayFromZero);
                    return string.Format("{0:#}", elevationDiff);
                }
                else
                {
                    var elevationDiff = ElevationLowInMeters - ElevationHighInMeters;
                    var elevationDiffInFeet = elevationDiff * 3.2808399;
                    elevationDiffInFeet = Math.Round(elevationDiffInFeet, 0, MidpointRounding.AwayFromZero);
                    return string.Format("{0:#}", elevationDiffInFeet);
                }
            } 
        }
        
        public string ElevationDifferenceUnit 
        { 
            get 
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                    return "m";
                else
                    return "ft";
            } 
        }

        [DataMember(Name = "start_latlong")]
        public float[] StartLatLong { get; set; }

        [DataMember(Name = "end_latlong")]
        public float[] EndLatLong { get; set; }

        [DataMember(Name = "climb_category")]
        public int ClimbCategory { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        public string CityAndState
        {
            get
            {
                var city = City;
                var state = State;
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state)) return string.Empty;
                if (string.IsNullOrWhiteSpace(city)) return state;
                if (string.IsNullOrWhiteSpace(state)) return city;
                if (city.ToUpper().Contains(state.ToUpper()))
                {
                    var index = city.ToUpper().IndexOf(state.ToUpper());
                    city = city.Remove(index);
                }
                return string.Format("{0}, {1}", city, state);
            }
        }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "private")]
        public bool Private { get; set; }

        [DataMember(Name = "starred")]
        public bool Starred { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updated_at")]
        public string UpdatedAt { get; set; }

        [DataMember(Name = "total_elevation_gain")]
        public float TotalElevationGain { get; set; }

        [DataMember(Name = "map")]
        public Strava.Maps.Map Map { get; set; }

        [DataMember(Name = "effort_count")]
        public int EffortCount { get; set; }

        [DataMember(Name = "athlete_count")]
        public int AthleteCount { get; set; }

        [DataMember(Name = "hazardous")]
        public bool Hazardous { get; set; }

        [DataMember(Name = "star_count")]
        public int StarCount { get; set; }
    }
}
