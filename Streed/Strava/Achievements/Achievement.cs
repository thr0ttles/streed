using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Achievements
{
    [DataContract]
    public class Achievement
    {
        [DataMember(Name = "type_id")]
        public int TypeId { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
        
        [DataMember(Name = "rank")]
        public int Rank { get; set; }
    }
}
