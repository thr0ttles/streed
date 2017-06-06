using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Segments
{
    [DataContract]
    public class Leaderboard
    {
        [DataMember(Name = "entry_count")]
        public int EntryCount { get; set; }

        [DataMember(Name = "entries")]
        public LeaderboardEffort[] Entries { get; set; }
    }
}
