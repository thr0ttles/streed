using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Activities
{
    [DataContract]
    public class DistributionBucket
    {
        [DataMember(Name="max")]
        public int Max { get; set; }

        [DataMember(Name="min")]
        public int Min { get; set; }

        [DataMember(Name = "time")]
        public int Time { get; set; }
    }
}
