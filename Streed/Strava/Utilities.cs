using Microsoft.Xna.Framework;
using Streed.Strava.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava
{
    public static class Utilities
    {
        public static bool AuthorizationFailed(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Response));

                try
                {
                    var response = serializer.ReadObject(ms);
                    if (response is Strava.Response)
                    {
                        var message = response as Strava.Response;
                        return (message.Message == "Authorization Error");
                    }
                }
                catch { }
            }
            return false;
        }

        public static string DistanceFromDistanceInMeters(double distanceInMeters)
        {
            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
            {
                var distanceInKilometers = distanceInMeters / 1000;
                distanceInKilometers = Math.Round(distanceInKilometers, 1, MidpointRounding.AwayFromZero);
                if (distanceInKilometers - (int)distanceInKilometers == 0)
                    return string.Format("{0:0}", distanceInKilometers);
                else
                    return string.Format("{0:0.0}", distanceInKilometers);
            }
            else
            {
                var distanceInMiles = distanceInMeters * 0.00062137119;
                distanceInMiles = Math.Round(distanceInMiles, 1, MidpointRounding.AwayFromZero);
                if (distanceInMiles - (int)distanceInMiles == 0)
                    return string.Format("{0:0}", distanceInMiles);
                else
                    return string.Format("{0:0.0}", distanceInMiles);
            }
        }

        public static string DistanceUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                    return "km";
                return "mi";
            }
        }

        public static string TimeFromTimeInSeconds(int elapsedTimeInSeconds)
        {
            var dt = new DateTime().AddSeconds(elapsedTimeInSeconds);
            if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
            if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
            return string.Format(":{0:ss}", dt);
        }

        public static string TotalElevationGainFromElevationGainInMeters(double totalElevationGainInMeters)
        {
            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
            {
                var elevation = Math.Round(totalElevationGainInMeters, 0, MidpointRounding.AwayFromZero);
                if ((int)elevation < 1) return string.Empty;
                return string.Format("{0:#}", elevation);
            }
            else
            {
                var elevation = totalElevationGainInMeters * 3.2808399;
                elevation = Math.Round(elevation, 0, MidpointRounding.AwayFromZero);
                if ((int)elevation < 1) return string.Empty;
                return string.Format("{0:#}", elevation);
            }
        }

        public static string TotalElevationGainUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    return "m";
                }
                else
                {
                    return "ft";
                }
            }
        }

        public static string SpeedFromSpeedInMetersPerSecond(double speedInMetersPerSecond)
        {
            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                return (speedInMetersPerSecond * 60 * 60 / 1000).ToString("0.0");
            else
                return (speedInMetersPerSecond * 0.00062137119 * 60 * 60).ToString("0.0");
        }

        public static string SpeedUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    return "kph";
                }
                else
                {
                    return "mph";
                }
            }
        }

        public static double TempFromTempInCelcius(double temp)
        {
            if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
            {
                return temp;
            }
            else
            {
                var tempInF = temp * 9 / 5 + 32;
                return tempInF;
            }
        }

        public static string TempUnit
        {
            get
            {
                if (DataAccess.StreedApplicationSettings.HasAccessToken &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete != null &&
                    DataAccess.StreedApplicationSettings.AccessToken.Athlete.MeasurementUnit == MeasurementType.Meters)
                {
                    return "C";
                }
                else
                {
                    return "F";
                }
            }
        }

        public static double CalculateDecoupling(double[] watts, int[] heartrate)
        {
            if (watts.Length != heartrate.Length) return 0;
            if (watts.Length == 0) return 0;

            var len = watts.Length;
            var half = Convert.ToInt32(len / 2);

            if (half == 0) return 0;

            var firstHalfWatts = new double[half];
            var secondHalfWatts = new double[half];
            var firstHalfHeartRateTotal = 0;
            var secondHalfHeartRateTotal = 0;

            for (var i = 0; i < half; i++)
            {
                firstHalfWatts[i] = watts[i];
                firstHalfHeartRateTotal += heartrate[i];
                secondHalfWatts[i] = watts[i + half];
                secondHalfHeartRateTotal += heartrate[i + half];
            }

            var firstHalfAvgWatts = CalculateAverageWatts(firstHalfWatts);
            var secondHalfAvgWatts = CalculateAverageWatts(secondHalfWatts);

            var firstHalfAvgHeartRate = firstHalfHeartRateTotal / half;
            var secondHalfAvgHeartRate = secondHalfHeartRateTotal / half;

            var firstHalfEF = CalculateEfficiencyFactor(firstHalfAvgWatts, firstHalfAvgHeartRate);
            var secondHalfEF = CalculateEfficiencyFactor(secondHalfAvgWatts, secondHalfAvgHeartRate);

            if (firstHalfEF == 0) return 0;
            return (firstHalfEF - secondHalfEF) / firstHalfEF * 100;
        }

        public static double CalculateEfficiencyFactor(double normalizedWatts, double averageHeartRate)
        {
            if (normalizedWatts <= 0 || averageHeartRate <= 0) return 0;
            return normalizedWatts / averageHeartRate;
        }

        public static double CalculateVariabilityIndex(double normalizedWatts, double averageWatts)
        {
            if (normalizedWatts <= 0 || averageWatts <= 0) return 0;
            return normalizedWatts / averageWatts;
        }

        public static double CalculateIntensityFactor(double averageWatts, int ftp)
        {
            if (ftp <= 0) return 0;
            return averageWatts / ftp;
        }

        public static double CalculateAverageWatts(double[] data)
        {
            if (data == null) return 0;
            if (data.Length == 0) return 0;

            var len = data.Length;
            var total = 0.0;

            for (var i = 0; i < len; i++)
                total += data[i];

            var avg = total / len;
            return avg;
        }

        public static double CalculateNormalizedWatts(double[] data)
        {
            if (data == null) return 0;
            if (data.Length < 30) return 0;

            var avgs = new List<double>();
            var len = data.Length;

            for (var high = 30; high < len; high++)
            {
                var low = high - 30;
                var total = 0.0;
                for (var i = low; i < high; i++)
                    total += data[i];
                var avg = total / 30;
                var raised = Math.Pow(avg, 4);
                avgs.Add(raised);
            }

            var lenOfAvgs = avgs.Count;
            var totalOfAvgs = avgs.Sum();
            var overallAvg = totalOfAvgs / lenOfAvgs;
            var normalizedWatts = Math.Pow(overallAvg, 0.25);
            return normalizedWatts;
        }

        public static double CalculateTSS(int timeInSeconds, double normalizedWatts, double intensityFactor, int ftp)
        {
            if (ftp <= 0) return 0;
            var tss = (timeInSeconds * normalizedWatts * intensityFactor) / (ftp * 3600) * 100;
            return tss;
        }

        public static double CalculateAverageHeartRate(int[] data)
        {
            if (data == null) return 0;
            if (data.Length == 0) return 0;

            var len = data.Length;
            var total = 0;

            for (var i = 0; i < len; i++)
                total += data[i];

            var avg = total / len;
            return avg;
        }

        public static IEnumerable<System.Device.Location.GeoCoordinate> PolylineToGeoCoordinates(string polyline)
        {
            var coordinates = new List<System.Device.Location.GeoCoordinate>();

            try
            {
                var index = 0;
                var leng = polyline.Length;

                double lat = 0;
                double lng = 0;

                while (index < leng)
                {
                    int b = 0;
                    int shift = 0;
                    int result = 0;

                    do
                    {
                        b = polyline.ElementAt(index++) - 63;
                        result |= ((b & 0x1f) << shift);
                        shift += 5;
                    } while (b >= 0x20);

                    double dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                    lat += dlat;

                    shift = 0;
                    result = 0;

                    do
                    {
                        b = polyline.ElementAt(index++) - 63;
                        result |= (b & 0x1f) << shift;
                        shift += 5;
                    } while (b >= 0x20);

                    double dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                    lng += dlng;

                    double actualLat, actualLng;
                    actualLat = lat * 1E-5;
                    actualLng = lng * 1E-5;

                    var p = new System.Device.Location.GeoCoordinate(actualLat, actualLng);
                    coordinates.Add(p);
                }
            }
            catch
            { }

            return coordinates;
        }

        public static string MapQuestPolylineFromGeoCoordinates(IEnumerable<System.Device.Location.GeoCoordinate> geoCoordinates)
        {
            Func<int, string> encodeNumber = delegate(int num)
            {
                var encodedString = string.Empty;
                while (num >= 0x20)
                {
                    encodedString += Convert.ToChar((0x20 | (num & 0x1f)) + 63);
                    num >>= 5;
                }
                encodedString += Convert.ToChar(num + 63);
                return encodedString;
            };

            Func<int, string> encodeSignedNumber = delegate(int num)
            {
                var sgn_num = num << 1;
                if (num < 0)
                    sgn_num = ~(sgn_num);
                return encodeNumber(sgn_num);            
            };

            var polyline = string.Empty;
            var plat = 0;
            var plng = 0;
            var encoded = string.Empty;

            geoCoordinates.ToList().ForEach(g =>
                {
                    var lat = g.Latitude;
                    var lng = g.Longitude;
                    var late5 = 0;
                    var lnge5 = 0;
                    late5 = Convert.ToInt32(Math.Floor(lat * 1e5));
                    lnge5 = Convert.ToInt32(Math.Floor(lng * 1e5));
                    var dlat = late5 - plat;
                    var dlng = lnge5 - plng;
                    plat = late5;
                    plng = lnge5;
                    polyline += encodeSignedNumber(dlat) + encodeSignedNumber(dlng);
                });

            return polyline;
        }

        public static string LatLngPairsFromGeoCoordinates(IEnumerable<System.Device.Location.GeoCoordinate> geoCoordinates)
        {
            var latlngPairs = string.Empty;
            if (geoCoordinates != null && geoCoordinates.Count() != 0)
            { 
                var sb = new StringBuilder();
                geoCoordinates.ToList().ForEach(g => sb.AppendFormat("{0},{1},", g.Latitude, g.Longitude));
                latlngPairs = sb.ToString().Substring(0, sb.Length - 1);
            }
            return latlngPairs;
        }

        public static string BoundingRectangleFromGeoCoordinates(IEnumerable<System.Device.Location.GeoCoordinate> geoCoordinates)
        {
            var boundingRectangle = string.Empty;
            if (geoCoordinates != null && geoCoordinates.Count() != 0)
            {
                var minLat = geoCoordinates.Min(m => m.Latitude);
                var minLng = geoCoordinates.Min(m => m.Longitude);
                var maxLat = geoCoordinates.Max(m => m.Latitude);
                var maxLng = geoCoordinates.Max(m => m.Longitude);

                boundingRectangle = string.Format("{0},{1},{2},{3}", minLat, minLng, maxLat, maxLng);
            }
            return boundingRectangle;
        }
    }
}
