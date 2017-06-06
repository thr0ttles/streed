using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Models
{
    public sealed class HeartRateZoneData
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

        public HeartRateZoneData(string category, string categoryDescription, string range, int timeInSeconds, int value)
        {
            Category = category;
            CategoryDescription = categoryDescription;
            Range = range;
            TimeInSeconds = timeInSeconds;
            Value = value;
        }

        public static List<HeartRateZoneData> DistributionBucketsToHeartRateData(Strava.Activities.DistributionBucket[] distributionBuckets, int activityMovingTimeInSeconds, int maxHeartRate)
        {
            var heartRateData = new List<HeartRateZoneData>();
            if (activityMovingTimeInSeconds <= 0 || distributionBuckets == null || distributionBuckets.Count() == 0 || distributionBuckets.Count() != 5)
                return heartRateData;

            heartRateData.Add(new HeartRateZoneData("Z1", "Endurance", string.Format("< {0}", distributionBuckets[0].Max), distributionBuckets[0].Time, (int)(Decimal.Divide(distributionBuckets[0].Time, activityMovingTimeInSeconds) * 100)));
            heartRateData.Add(new HeartRateZoneData("Z2", "Moderate", string.Format("{0} - {1}", distributionBuckets[1].Min, distributionBuckets[1].Max),distributionBuckets[1].Time, (int)(Decimal.Divide(distributionBuckets[1].Time, activityMovingTimeInSeconds) * 100)));
            heartRateData.Add(new HeartRateZoneData("Z3", "Tempo", string.Format("{0} - {1}", distributionBuckets[2].Min, distributionBuckets[2].Max), distributionBuckets[2].Time, (int)(Decimal.Divide(distributionBuckets[2].Time, activityMovingTimeInSeconds) * 100)));
            heartRateData.Add(new HeartRateZoneData("Z4", "Threshold", string.Format("{0} - {1}", distributionBuckets[3].Min, distributionBuckets[3].Max), distributionBuckets[3].Time, (int)(Decimal.Divide(distributionBuckets[3].Time, activityMovingTimeInSeconds) * 100)));
            
            if (maxHeartRate != 0)
                heartRateData.Add(new HeartRateZoneData("Z5", "Anaerobic", string.Format("{0} - {1}", distributionBuckets[4].Min, (distributionBuckets[4].Max == -1 ? maxHeartRate : distributionBuckets[4].Max)), distributionBuckets[4].Time, (int)(Decimal.Divide(distributionBuckets[4].Time, activityMovingTimeInSeconds) * 100)));
            else
                heartRateData.Add(new HeartRateZoneData("Z5", "Anaerobic", string.Format("> {0}", distributionBuckets[4].Min), distributionBuckets[4].Time, (int)(Decimal.Divide(distributionBuckets[4].Time, activityMovingTimeInSeconds) * 100)));
            return heartRateData;
        }
    }
}
