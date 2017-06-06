using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class Segments
    {
        public static async Task<Strava.Segments.SegmentEffort> GetSegmentEffort(long id, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/segment_efforts/:id

            var result = await RestRequest.Get(string.Format("api/v3/segment_efforts/{0}", id), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Segments.SegmentEffort));
                    return (Strava.Segments.SegmentEffort)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Segments.Leaderboard> GetSegmentLeaderboard(long id, string accessToken, string gender = null, bool thisYear = false)
        {
            ///GET https://www.strava.com/api/v3/segments/:id/leaderboard

            var parameters = new List<RestSharp.Parameter>();
            if (thisYear) parameters.Add(new RestSharp.Parameter { Name = "date_range", Value = "this_year", Type = RestSharp.ParameterType.GetOrPost });
            if (string.IsNullOrWhiteSpace(gender) == false) parameters.Add(new RestSharp.Parameter { Name = "gender", Value = gender, Type = RestSharp.ParameterType.GetOrPost });

            var result = await RestRequest.Get(string.Format("api/v3/segments/{0}/leaderboard", id), accessToken, parameters);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Segments.Leaderboard));
                    return (Strava.Segments.Leaderboard)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Segments.Segment> GetSegment(long id, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/segments/:id

            var result = await RestRequest.Get(string.Format("api/v3/segments/{0}", id), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Segments.Segment));
                    return (Strava.Segments.Segment)serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
