using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public class RateLimitExceededException : Exception
    {
        public RateLimit RateLimit { get; private set; }

        public RateLimitExceededException(RateLimit rateLimit)
        {
            RateLimit = rateLimit;
        }
    }
}
