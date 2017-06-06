using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Models
{
    public class ElevationData
    {
        public double Elevation { get; set; }
        public double Distance { get; set; }
        public int Time { get; set; }
        public double Gradient { get; set; }

        public ElevationData(double distance, double elevation, int time, double gradient)
        {
            Distance = distance;
            Elevation = elevation;
            Time = time;
            Gradient = gradient;
        }

        public static bool AreStreamsValid(IEnumerable<Strava.Streams.Stream> streams)
        {
            if (streams == null)
                return false;

            if (streams.Where(w => w == null).Any())
                return false;

            if (streams.Where(w => w.Data == null).Any())
                return false;

            if (streams.Where(w => w.Data.Length == 0).Any())
                return false;

            var len = streams.First().Data.Length;
            var a = streams.Select(s => s.Data.Length).ToArray();
            if (false == Array.TrueForAll<int>(a, val => val == len))
                return false;

            return true;
        }

        public static List<ElevationData> StreamsToElevationData(IEnumerable<Strava.Streams.Stream> streams, Strava.MeasurementType measurementType)
        {
            //data is in meters, convert to measurementType, if needed
            var elevationData = new List<ElevationData>();

            if (false == AreStreamsValid(streams))
                return elevationData;

            if (streams.Count() != 4)
                return elevationData;

            var distanceStream = streams.Where(w => w.Type == "distance").SingleOrDefault();
            if (distanceStream == null)
                return elevationData;

            var altitudeStream = streams.Where(w => w.Type == "altitude").SingleOrDefault();
            if (altitudeStream == null)
                return elevationData;

            var timeStream = streams.Where(w => w.Type == "time").SingleOrDefault();
            if (timeStream == null)
                return elevationData;

            var gradientStream = streams.Where(w => w.Type == "grade_smooth").SingleOrDefault();
            if (gradientStream == null)
                return elevationData;

            var dataPointCount = distanceStream.Data.Count();
            var distanceData = distanceStream.Data;
            var altitudeData = altitudeStream.Data;
            var timeData = timeStream.Data;
            var gradientData = gradientStream.Data;
            
            var convertToStandard = (measurementType == Strava.MeasurementType.Feet);

            for (var i = 0; i < dataPointCount; i++)
            {
                var distance = Convert.ToDouble(distanceData[i]);
                var altitude = Convert.ToDouble(altitudeData[i]);
                var time = Convert.ToInt32(timeData[i]);
                var gradient = Convert.ToDouble(gradientData[i]);

                if (convertToStandard)
                {
                    distance = distance * 0.00062137119;
                    altitude = altitude * 3.2808399;
                }

                distance = Math.Round(distance, 1, MidpointRounding.AwayFromZero);
                altitude = Math.Round(altitude, 1, MidpointRounding.AwayFromZero);

                //don't repeat any data. could happen since we are converting and rounding.
                if (elevationData.Where(w => w.Distance == distance).Any())
                    continue;

                elevationData.Add(new ElevationData(distance, altitude, time, gradient));
            }

            return elevationData;
        }
    }
}
