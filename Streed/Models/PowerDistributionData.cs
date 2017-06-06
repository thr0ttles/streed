using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Models
{
    public sealed class PowerDistributionData
    {
        public int Time { get; private set; }
        public string Power { get; private set; }

        private PowerDistributionData()
        {
        }

        public static List<PowerDistributionData> StreamToPowerDistributionData(Strava.Streams.Stream stream)
        {
            var powerDistributionData = new List<PowerDistributionData>();
            if (stream == null && stream.Data == null)
                return powerDistributionData;

            if (stream.Data.Any() == false)
                return powerDistributionData;

            var maxPower = Convert.ToInt32(stream.Data.Max());
            var diff = maxPower % 25;
            var max = maxPower + (25 - diff);
            var buckets = max / 25;
            var powerBuckets = new dynamic[buckets];
            
            for (var i = 0; i < buckets; i ++)
                powerBuckets[i] = new { Bucket = (i * 25).ToString(), Count = 0 };

            stream.Data.ToList().ForEach(dp =>
                {
                    var datapoint = Convert.ToDecimal(dp);
                    var bucket = (int)Decimal.Divide(datapoint, 25);
                    powerBuckets[bucket] = new { Bucket = powerBuckets[bucket].Bucket, Count = powerBuckets[bucket].Count + 1 };
                });

            //filter out some buckets for sake of the chart control
            var threshold = (stream.Type == "watts" ? 6 : 31);

            for (var i = 0; i < buckets; i++)
            {
                var pb = powerBuckets[i];
                //zero out buckets with less than threshold (sec)
                if (pb.Count < threshold) 
                    powerBuckets[i] = new { Bucket = pb.Bucket, Count = 0 };
            };

            var tempPowerBuckets = new List<dynamic>();
            powerBuckets.ToList().ForEach(pb => 
                {
                    if (pb.Count != 0) tempPowerBuckets.Add(new { Bucket = pb.Bucket, Count = pb.Count });
                });

            tempPowerBuckets.ForEach(pb =>
                {
                    powerDistributionData.Add(new PowerDistributionData 
                        { 
                            Power = pb.Bucket,
                            Time = pb.Count
                        });
                });

            return powerDistributionData;
        }
    }
}
