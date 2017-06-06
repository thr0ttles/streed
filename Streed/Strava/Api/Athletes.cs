using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class Athletes
    {
        public static async Task<Strava.Athletes.Athlete> GetAthlete(long id, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/athletes/:id/

            var result = await RestRequest.Get(string.Format("api/v3/athletes/{0}", id), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Athletes.Athlete));
                    return (Strava.Athletes.Athlete)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Athletes.Athlete> GetCurrentAthlete(string accessToken)
        {
            ///GET https://www.strava.com/api/v3/athlete/

            var result = await RestRequest.Get("api/v3/athlete", accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Athletes.Athlete));
                    return (Strava.Athletes.Athlete)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Segments.SegmentEffort[]> GetKoms(long id, string accessToken, int currentPage = 0, int perPage = 20)
        {
            ///GET https://www.strava.com/api/v3/athletes/:id/koms


            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get(string.Format("api/v3/athletes/{0}/koms", id), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Segments.SegmentEffort[]));
                    return (Strava.Segments.SegmentEffort[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Athletes.Athlete[]> GetFriends(long id, string accessToken, int currentPage = 0, int perPage = 200)
        {
            ///GET https://www.strava.com/api/v3/athletes/:id/friends

            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get(string.Format("api/v3/athletes/{0}/friends", id), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Athletes.Athlete[]));
                    return (Strava.Athletes.Athlete[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Athletes.Athlete[]> GetFollowers(long id, string accessToken, int currentPage = 0, int perPage = 200)
        {
            ///GET https://www.strava.com/api/v3/athletes/:id/followers


            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get(string.Format("api/v3/athletes/{0}/followers", id), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Athletes.Athlete[]));
                    return (Strava.Athletes.Athlete[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Athletes.Athlete[]> GetBothFollowing(long id, string accessToken, int currentPage = 0, int perPage = 200)
        {
            ///GET https://www.strava.com/api/v3/athletes/:id/both-following

            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get(string.Format("api/v3/athletes/{0}/both-following", id), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Athletes.Athlete[]));
                    return (Strava.Athletes.Athlete[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
