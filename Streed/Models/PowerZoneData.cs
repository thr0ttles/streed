using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Models
{
    public sealed class PowerZoneData
    {
        public string Category { get; private set; }
        public string CategoryDescription { get; private set; }
        public string Range { get; private set; }
        public int TimeInSeconds { get; private set; }
        public int Value { get; private set; }
        public string Time 
        {
            get
            {
                var dt = new DateTime().AddSeconds(TimeInSeconds);
                if (dt.Hour > 0) return string.Format("{0:%h}:{1:mm}:{2:ss}", dt, dt, dt);
                if (dt.Minute > 0) return string.Format("{0:mm}:{1:ss}", dt, dt);
                return string.Format(":{0:ss}", dt);
            }
        }

        private PowerZoneData() { }

        public static List<PowerZoneData> StreamToPowerZoneData(Strava.Streams.Stream stream, int ftp)
        {
            var powerZoneData = new List<PowerZoneData>();
            if (stream == null || ftp == 0)
                return powerZoneData;

            //determine 7 power zones based on ftp
            var z1 = ftp * .55;
            var z2 = ftp * .75;
            var z3 = ftp * .90;
            var z4 = ftp * 1.05;
            var z5 = ftp * 1.2;
            var z6 = ftp * 1.5;
            z1 = Math.Round(z1, 0, MidpointRounding.AwayFromZero);
            z2 = Math.Round(z2, 0, MidpointRounding.AwayFromZero);
            z3 = Math.Round(z3, 0, MidpointRounding.AwayFromZero);
            z4 = Math.Round(z4, 0, MidpointRounding.AwayFromZero);
            z5 = Math.Round(z5, 0, MidpointRounding.AwayFromZero);
            z6 = Math.Round(z6, 0, MidpointRounding.AwayFromZero);

            var powerBuckets = new int[7];

            //iterate through data and place into buckets (power zones)
            stream.Data.ToList().ForEach(dp =>
                {
                    var datapoint = Convert.ToInt32(dp);
                    if (datapoint > z6) powerBuckets[6]++;
                    else if (datapoint > z5 && datapoint <= z6) powerBuckets[5]++;
                    else if (datapoint > z4 && datapoint <= z5) powerBuckets[4]++;
                    else if (datapoint > z3 && datapoint <= z4) powerBuckets[3]++;
                    else if (datapoint > z2 && datapoint <= z3) powerBuckets[2]++;
                    else if (datapoint > z1 && datapoint <= z2) powerBuckets[1]++;
                    else powerBuckets[0]++;
                });

            //calculate % time in each zone
            powerZoneData.Add(new PowerZoneData 
                {
                    Category = "Z1", 
                    CategoryDescription = "Recovery", 
                    Range = string.Format("1 - {0} W", z1), 
                    TimeInSeconds = powerBuckets[0], 
                    Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[0]), Convert.ToDecimal(stream.OriginalSize)) * 100)
                });
            powerZoneData.Add(new PowerZoneData
                {
                    Category = "Z2", 
                    CategoryDescription = "Endurance", 
                    Range = string.Format("{0} - {1} W", z1, z2), 
                    TimeInSeconds = powerBuckets[1],
                    Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[1]), Convert.ToDecimal(stream.OriginalSize)) * 100)
                });
            powerZoneData.Add(new PowerZoneData
            {
                Category = "Z3",
                CategoryDescription = "Tempo",
                Range = string.Format("{0} - {1} W", z2, z3),
                TimeInSeconds = powerBuckets[2],
                Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[2]), Convert.ToDecimal(stream.OriginalSize)) * 100)
            });
            powerZoneData.Add(new PowerZoneData
            {
                Category = "Z4",
                CategoryDescription = "Threshold",
                Range = string.Format("{0} - {1} W", z3, z4),
                TimeInSeconds = powerBuckets[3],
                Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[3]), Convert.ToDecimal(stream.OriginalSize)) * 100)
            });
            powerZoneData.Add(new PowerZoneData
            {
                Category = "Z5",
                CategoryDescription = "VO2Max",
                Range = string.Format("{0} - {1} W", z4, z5),
                TimeInSeconds = powerBuckets[4],
                Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[4]), Convert.ToDecimal(stream.OriginalSize)) * 100)
            });
            powerZoneData.Add(new PowerZoneData
            {
                Category = "Z6",
                CategoryDescription = "Anaerobic",
                Range = string.Format("{0} - {1} W", z5, z6),
                TimeInSeconds = powerBuckets[5],
                Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[5]), Convert.ToDecimal(stream.OriginalSize)) * 100)
            });
            powerZoneData.Add(new PowerZoneData
            {
                Category = "Z7",
                CategoryDescription = "Sprint",
                Range = string.Format("{0}+ W", z6),
                TimeInSeconds = powerBuckets[6],
                Value = (int)(Decimal.Divide(Convert.ToDecimal(powerBuckets[6]), Convert.ToDecimal(stream.OriginalSize)) * 100)
            });

            return powerZoneData;
        }
    }
}
