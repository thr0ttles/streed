using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Streams
{
    [DataContract]
    public class Stream
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "data")]
        public object[] Data { get; set; }

        [DataMember(Name = "series_type")]
        public string SeriesType { get; set; }

        [DataMember(Name = "original_size")]
        public int OriginalSize { get; set; }

        [DataMember(Name = "resolution")]
        public string Resolution { get; set; }
    }
}
