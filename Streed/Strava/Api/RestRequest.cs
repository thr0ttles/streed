using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class RestRequest
    {
        public static DateTime ServerDateTime { get; private set; }
        public static RateLimit RateLimit { get; private set; }

        public static async Task<string> Get(string uri, string accessToken, IEnumerable<RestSharp.Parameter> parameters = null)
        {
            return await MakeRequest(uri, accessToken, RestSharp.Method.GET, parameters);
        }

        public static async Task<string> Post(string uri, string accessToken, IEnumerable<RestSharp.Parameter> parameters = null)
        {
            return await MakeRequest(uri, accessToken, RestSharp.Method.POST, parameters);
        }

        public static async Task<string> Delete(string uri, string accessToken, IEnumerable<RestSharp.Parameter> parameters = null)
        {
            return await MakeRequest(uri, accessToken, RestSharp.Method.DELETE, parameters);
        }

        public static async Task<string> Put(string uri, string accessToken, IEnumerable<RestSharp.Parameter> parameters = null)
        {
            return await MakeRequest(uri, accessToken, RestSharp.Method.PUT, parameters);
        }

        private static async Task<string> MakeRequest(string uri, string accessToken, RestSharp.Method method, IEnumerable<RestSharp.Parameter> parameters = null)
        {
#if DEBUG
            var sb = new StringBuilder();
            if (parameters != null && parameters.Count() != 0)
                parameters.ToList().ForEach(p => sb.AppendLine(string.Format("{0}:{1}", p.Name, p.Value)));
            System.Diagnostics.Debugger.Log(0, "api", 
                string.Format("Api Request: uri:{0}\n\taccessToken:{1}\n\tmethod:{2}\nparameters:\n{3}\n", 
                    uri, 
                    accessToken, 
                    method.ToString(), 
                    sb.ToString()));
#endif
            var request = new RestSharp.RestRequest(uri, method);
            if (string.IsNullOrWhiteSpace(accessToken) == false) 
                request.AddParameter("access_token", accessToken);
            if (parameters != null && parameters.Count() != 0)
                parameters.ToList().ForEach(p => request.AddParameter(p));
            request.RequestFormat = RestSharp.DataFormat.Json;

            var client = new RestSharp.RestClient("https://www.strava.com");

            RestSharp.IRestResponse r = null;

            if (method == RestSharp.Method.GET)
                r = await RestSharpExtensions.ExecuteGetAwait(client, request);
            else if (method == RestSharp.Method.POST)
                r = await RestSharpExtensions.ExecutePostAwait(client, request);
            else if (method == RestSharp.Method.PUT)
                r = await RestSharpExtensions.ExecutePutAwait(client, request);
            else
                throw new NotImplementedException();

            if (r.Headers.Where(w => w.Name == "Date").Any())
            {
                var serverDate = (string)r.Headers.Where(w => w.Name == "Date").Select(s => s.Value).SingleOrDefault();
                DateTime dt;
                if (DateTime.TryParse(serverDate, out dt))
                    ServerDateTime = dt.ToUniversalTime();
            }

            if (r.Headers.Where(w => w.Name == "X-RateLimit-Limit").Any() &&
                r.Headers.Where(w => w.Name == "X-RateLimit-Usage").Any())
            {
                var rateLimits = (string)r.Headers.Where(w => w.Name == "X-RateLimit-Limit").Select(s => s.Value).SingleOrDefault();
                var rateUsuages = (string)r.Headers.Where(w => w.Name == "X-RateLimit-Usage").Select(s => s.Value).SingleOrDefault();

                rateLimits = rateLimits.Trim();
                rateUsuages = rateUsuages.Trim();
                var idx1 = rateLimits.IndexOf(',');
                var idx2 = rateUsuages.IndexOf(',');
                if (idx1 != -1 && idx2 != -1)
                {
                    var stl = Convert.ToInt64(rateLimits.Substring(0, idx1));
                    var ltl = Convert.ToInt64(rateLimits.Substring(idx1 + 1));
                    var stu = Convert.ToInt64(rateUsuages.Substring(0, idx2));
                    var ltu = Convert.ToInt64(rateUsuages.Substring(idx2 + 1));

                    RateLimit = new RateLimit(stl, ltl, stu, ltu);

                    if (RateLimit.IsShortTermLimitExceeded || RateLimit.IsLongTermLimitExceeded)
                        throw new RateLimitExceededException(RateLimit);
                }
            }

            if (string.IsNullOrWhiteSpace(r.Content))
                return null;

            if (Strava.Utilities.AuthorizationFailed(r.Content))
                throw new Strava.Api.AuthorizationFailedException();

            //TODO: check for error (refactor AuthorizationFailed method from above)

            return r.Content;
        }
    }
}
