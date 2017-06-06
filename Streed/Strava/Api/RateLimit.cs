using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public sealed class RateLimit
    {
        public long ShortTermLimit { get; private set; }
        public long LongTermLimit { get; private set; }

        public long ShortTermUsage { get; private set; }
        public long LongTermUsage { get; private set; }

        public RateLimit()
        {
            ShortTermLimit = 600;
            LongTermLimit = 30000;
            ShortTermUsage = 0;
            LongTermUsage = 0;
        }

        public RateLimit(long shorttermlimit, long longtermlimit, long shorttermusuage, long longtermusuage)
        {
            ShortTermLimit = shorttermlimit;
            LongTermLimit = longtermlimit;
            ShortTermUsage = shorttermusuage;
            LongTermUsage = longtermusuage;
        }

        public bool IsShortTermLimitExceeded
        {
            get
            {
                return (ShortTermUsage >= ShortTermLimit);
            }
        }

        public bool IsLongTermLimitExceeded
        {
            get
            {
                return (LongTermUsage >= LongTermLimit);
            }
        }
    }
}
