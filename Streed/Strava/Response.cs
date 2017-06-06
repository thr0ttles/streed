using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Streed.Strava
{
    [DataContract]
    public class Response
    {
        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "errors")]
        public Error[] Errors { get; set; }
    }
}
