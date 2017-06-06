using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class Activities
    {
        public static async Task<Strava.Activities.Activity> GetActivity(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/

            var parameters = new List<RestSharp.Parameter>();
            parameters.Add(new RestSharp.Parameter { Name = "include_all_efforts", Value = true, Type = RestSharp.ParameterType.GetOrPost });

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}", activityId), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                    return (Strava.Activities.Activity)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Activities.Activity> UpdateActivity(long activityId, string accessToken, string name, string description, bool isPrivate)
        {
            ///PUT https://www.strava.com/api/v3/activities/:id/

            var parameters = new List<RestSharp.Parameter>();
            parameters.Add(new RestSharp.Parameter { Name = "name", Value = name, Type = RestSharp.ParameterType.GetOrPost });
            parameters.Add(new RestSharp.Parameter { Name = "description", Value = description, Type = RestSharp.ParameterType.GetOrPost });
            parameters.Add(new RestSharp.Parameter { Name = "private", Value = isPrivate, Type = RestSharp.ParameterType.GetOrPost });

            var result = await RestRequest.Put(string.Format("api/v3/activities/{0}", activityId), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                    return (Strava.Activities.Activity)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task GiveKudos(long activityId, string accessToken)
        {
            ///POST https://www.strava.com/api/v3/activities/:id/kudos

            await RestRequest.Post(string.Format("api/v3/activities/{0}/kudos", activityId), accessToken);
        }

        public static async Task<Strava.Activities.Activity[]> GetRelatedActivities(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/related

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/related", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Activity[]));
                    return (Strava.Activities.Activity[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Activities.ActivityZone[]> GetZones(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/zones

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/zones", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.ActivityZone[]));
                    return (Strava.Activities.ActivityZone[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Activities.Activity[]> GetFriendActivities(string accessToken, int currentPage = 0, int perPage = 20)
        {
            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get("api/v3/activities/following", accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Activity[]));
                    return (Strava.Activities.Activity[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Activities.Activity[]> GetAthleteActivities(string accessToken, int currentPage = 0, int perPage = 20)
        {
            var parameters = new List<RestSharp.Parameter>();
            if (currentPage != 0)
            {
                parameters.Add(new RestSharp.Parameter { Name = "page", Value = currentPage, Type = RestSharp.ParameterType.GetOrPost });
                parameters.Add(new RestSharp.Parameter { Name = "per_page", Value = perPage, Type = RestSharp.ParameterType.GetOrPost });
            }

            var result = await RestRequest.Get("api/v3/athlete/activities", accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Activity[]));
                    return (Strava.Activities.Activity[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Comments.Comment[]> GetComments(long activityId, string accessToken, int currentPage = 0, int perPage = 20)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/comments

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/comments", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Comments.Comment[]));
                    return (Strava.Comments.Comment[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Comments.Comment> InsertComment(long activityId, string accessToken, string text)
        {
            ///POST https://www.strava.com/api/v3/activities/:id/comments

            var parameters = new List<RestSharp.Parameter>();
            parameters.Add(new RestSharp.Parameter { Name = "id", Value = activityId, Type = RestSharp.ParameterType.GetOrPost });
            parameters.Add(new RestSharp.Parameter { Name = "text", Value = text, Type = RestSharp.ParameterType.GetOrPost });

            var result = await RestRequest.Post(string.Format("api/v3/activities/{0}/comments", activityId), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Comments.Comment));
                    return (Strava.Comments.Comment)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Activities.Lap[]> GetLaps(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/laps

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/laps", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Activities.Lap[]));
                    return (Strava.Activities.Lap[])serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
