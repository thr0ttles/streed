using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class Authentication
    {
        public static async Task<Strava.Authorization.AccessToken> Authorize(string clientId, string clientSecret, string code)
        {
            var parameters = new List<RestSharp.Parameter>();
            parameters.Add(new RestSharp.Parameter { Name = "client_id", Value = clientId, Type = RestSharp.ParameterType.GetOrPost });
            parameters.Add(new RestSharp.Parameter { Name = "client_secret", Value = clientSecret, Type = RestSharp.ParameterType.GetOrPost });
            parameters.Add(new RestSharp.Parameter { Name = "code", Value = code, Type = RestSharp.ParameterType.GetOrPost });

            var result = await RestRequest.Post("oauth/token", null, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Authorization.AccessToken));
                    return (Strava.Authorization.AccessToken)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task Deauthorize(string accessToken)
        {
            await RestRequest.Post("oauth/deauthorize", accessToken);
        }
    }
}
