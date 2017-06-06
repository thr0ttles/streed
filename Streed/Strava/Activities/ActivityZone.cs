using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Activities
{
    [DataContract]
    public class ActivityZone
    {
        [DataMember(Name="score")]
        public int Score { get; set; }

        [DataMember(Name="distribution_buckets")]
        public DistributionBucket[] DistributionBuckets { get; set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="resource_state")]
        public int ResourceState { get; set; }
        
        [DataMember(Name="sensor_based")]
        public bool SensorBased { get; set; }
        
        [DataMember(Name="points")]
        public int Points { get; set; }
        
        [DataMember(Name="custom_zones")]
        public bool CustomZones { get; set; }
        
        [DataMember(Name="max")]
        public int Max { get; set; }
    }
}
