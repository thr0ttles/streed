using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Api
{
    public static class Streams
    {
        public static async Task<Strava.Streams.Stream[]> GetAllStreams(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/watts,watts_calc,heartrate,altitude,time,grade_smooth

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/watts,watts_calc,heartrate,altitude,time,grade_smooth", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    var wattsStream = streams.Where(w => w.Type == "watts").SingleOrDefault();
                    var wattsCalcStream = streams.Where(w => w.Type == "watts_calc").SingleOrDefault();
                    var heartrateStream = streams.Where(w => w.Type == "heartrate").SingleOrDefault();
                    var distanceStream = streams.Where(w => w.Type == "distance").SingleOrDefault();
                    var altitudeStream = streams.Where(w => w.Type == "altitude").SingleOrDefault();
                    var timeStream = streams.Where(w => w.Type == "time").SingleOrDefault();
                    var gradientStream = streams.Where(w => w.Type == "grade_smooth").SingleOrDefault();

                    var s = new List<Strava.Streams.Stream>();
                    if (wattsStream != null) s.Add(wattsStream);
                    if (wattsCalcStream != null) s.Add(wattsCalcStream);
                    if (heartrateStream != null) s.Add(heartrateStream);
                    if (distanceStream != null) s.Add(distanceStream);
                    if (altitudeStream != null) s.Add(altitudeStream);
                    if (timeStream != null) s.Add(timeStream);
                    if (gradientStream != null) s.Add(gradientStream);
                    return s.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Streams.Stream[]> GetAllStreamsExcludingAltitude(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/watts,watts_calc,heartrate

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/watts,watts_calc,heartrate", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    var wattsStream = streams.Where(w => w.Type == "watts").SingleOrDefault();
                    var wattsCalcStream = streams.Where(w => w.Type == "watts_calc").SingleOrDefault();
                    var heartrateStream = streams.Where(w => w.Type == "heartrate").SingleOrDefault();
                    var distanceStream = streams.Where(w => w.Type == "distance").SingleOrDefault();

                    var s = new List<Strava.Streams.Stream>();
                    if (wattsStream != null) s.Add(wattsStream);
                    if (wattsCalcStream != null) s.Add(wattsCalcStream);
                    if (heartrateStream != null) s.Add(heartrateStream);
                    if (distanceStream != null) s.Add(distanceStream);
                    return s.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Streams.Stream> GetWatts(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/watts

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/watts", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    return streams.Where(w => w.Type == "watts").SingleOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public async static Task<Strava.Streams.Stream> GetWattsCalc(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/watts_calc


            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/watts_calc", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    return streams.Where(w => w.Type == "watts_calc").SingleOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Streams.Stream> GetHeartRate(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/heartrate

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/heartrate", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    return streams.Where(w => w.Type == "heartrate").SingleOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Strava.Streams.Stream[]> GetAltitude(long activityId, string accessToken)
        {
            ///GET https://www.strava.com/api/v3/activities/:id/streams/altitude

            var result = await RestRequest.Get(string.Format("api/v3/activities/{0}/streams/altitude,time", activityId), accessToken);

            try
            {
                using (var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Strava.Streams.Stream[]));
                    var streams = (Strava.Streams.Stream[])serializer.ReadObject(ms);
                    var distanceStream = streams.Where(w => w.Type == "distance").SingleOrDefault();
                    var altitudeStream = streams.Where(w => w.Type == "altitude").SingleOrDefault();
                    var timeStream = streams.Where(w => w.Type == "time").SingleOrDefault();
                    return new Strava.Streams.Stream[] { distanceStream, altitudeStream, timeStream };
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
