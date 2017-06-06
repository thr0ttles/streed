using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Streed.Strava.Activities
{
    [DataContract]
    public enum ActivityType
    {
        [EnumMember]
        Ride,
        [EnumMember]
        Run,
        [EnumMember]
        Swim,
        [EnumMember]
        Hike,
        [EnumMember]
        Walk,
        [EnumMember]
        AlpineSki,
        [EnumMember]
        BackcountrySki,
        [EnumMember]
        Canoeing,
        [EnumMember]
        CrossCountrySkiing,
        [EnumMember]
        Crossfit,
        [EnumMember]
        Elliptical,
        [EnumMember]
        IceSkate,
        [EnumMember]
        InlineSkate,
        [EnumMember]
        Kayaking,
        [EnumMember]
        Kitesurf,
        [EnumMember]
        NordicSki,
        [EnumMember]
        RockClimbing,
        [EnumMember]
        RollerSki,
        [EnumMember]
        Rowing,
        [EnumMember]
        Snowboard,
        [EnumMember]
        Snowshoe,
        [EnumMember]
        StairStepper,
        [EnumMember]
        StandUpPaddling,
        [EnumMember]
        Surfing,
        [EnumMember]
        WeightTraining,
        [EnumMember]
        Windsurf,
        [EnumMember]
        Workout,
        [EnumMember]
        Yoga
    }
}
