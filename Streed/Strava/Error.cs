using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Streed.Strava
{
    [DataContract]
    public class Error
    {
        [DataMember(Name = "resource")]
        public string Resource { get; set; }

        [DataMember(Name = "field")]
        public string Field { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }
    }
}
