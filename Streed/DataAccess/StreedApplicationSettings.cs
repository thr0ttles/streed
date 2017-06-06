using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Streed.DataAccess
{
    public static class StreedApplicationSettings
    {
        public static string ClientId { get { return "3266"; } }
        public static string ClientSecret { get { return "28e8e7289ce805468b6eb69d3191c2abadce6a81"; } }

        public static long AuthenticatedAthleteId
        {
            get
            {
                if (HasAccessToken)
                {
                    return AccessToken.Athlete.Id;
                }
                return 0;
            }
        }

        public static Strava.Authorization.AccessToken AccessToken 
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("access_token"))
                {
                    return (Strava.Authorization.AccessToken)IsolatedStorageSettings.ApplicationSettings["access_token"];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("access_token");
                    IsolatedStorageSettings.ApplicationSettings.Remove("last_refresh_feed_datetime");
                    return;
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("access_token"))
                {
                    IsolatedStorageSettings.ApplicationSettings["access_token"] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("access_token", value);
                }
            }
        }

        public static bool HasAccessToken
        {
            get
            {
                if (System.ComponentModel.DesignerProperties.IsInDesignTool) return false;
                return IsolatedStorageSettings.ApplicationSettings.Contains("access_token");
            }
        }

        public static DateTime LastTimeRefreshedFeed
        {
            get
            { 
                if (IsolatedStorageSettings.ApplicationSettings.Contains("last_refresh_feed_datetime"))
                {
                    return (DateTime)IsolatedStorageSettings.ApplicationSettings["last_refresh_feed_datetime"];
                }
                return DateTime.MinValue.ToUniversalTime();
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("last_refresh_feed_datetime"))
                {
                    IsolatedStorageSettings.ApplicationSettings["last_refresh_feed_datetime"] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("last_refresh_feed_datetime", value);
                }
            }
        }

        public static bool ShouldRefreshFeed
        {
            get
            {
                var threshold = 15.0;
                var result = (DateTime.UtcNow - LastTimeRefreshedFeed).TotalMinutes > threshold;
                return result;
            }
        }

        public static string RefreshedFeedAgo
        {
            get
            {
                var last = LastTimeRefreshedFeed;
                if (last == DateTime.MinValue.ToUniversalTime())
                    return string.Empty;

                var diff = DateTime.UtcNow - last;
                if ((int)diff.TotalDays > 730)
                {
                    return string.Format("{0} years ago", (int)(diff.TotalDays / 365));
                }
                else if ((int)diff.TotalDays > 365)
                {
                    return "over a year ago";
                }
                else if ((int)diff.TotalDays > 30)
                {
                    return string.Format("{0} months ago", (int)(diff.TotalDays / 30));
                }
                else if ((int)diff.TotalDays > 1)
                {
                    return string.Format("{0} days ago", (int)diff.TotalDays);
                }
                else if ((int)diff.TotalDays == 1)
                {
                    return "1 day ago";
                }
                else if ((int)diff.TotalHours > 1)
                {
                    return string.Format("{0} hours ago", (int)diff.TotalHours);
                }
                else if ((int)diff.TotalHours == 1)
                {
                    return "1 hour ago";
                }
                else if ((int)diff.TotalMinutes > 1)
                {
                    return string.Format("{0} minutes ago", (int)diff.TotalMinutes);
                }
                else if ((int)diff.TotalMinutes == 1)
                {
                    return "over a minute ago";
                }
                else if ((int)diff.TotalSeconds < 15)
                {
                    return "a moment ago";
                }
                else
                {
                    if ((int)diff.TotalSeconds < 0)
                        return string.Empty;

                    return string.Format("{0} seconds ago", (int)diff.TotalSeconds);
                }
            }
        }

        public static bool IsFeedFilteringByFriends
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("feed_filtering_friends"))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings["feed_filtering_friends"];
                }
                return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("feed_filtering_friends"))
                {
                    IsolatedStorageSettings.ApplicationSettings["feed_filtering_friends"] = value;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("feed_filtering_friends", value);
                }
            }
        }

        public static string MapApplicationId { get { return "a0088a35-ac41-4440-b51f-b329ddf6170f"; } }
        public static string MapAuthenticationToken { get { return "JxebkBTPLYgHDWoNW_yLfw"; } }

        private static object _lock = new object();
        private static Streed.Models.SerializableException _exception;
        public static Streed.Models.SerializableException UnhandledException 
        {
            get
            {
                try
                {
                    Monitor.Enter(_lock);
                    if (_exception == null) return null;
                    var e = new Streed.Models.SerializableException(_exception);
                    _exception = null;
                    return e;
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            set
            {
                try
                {
                    Monitor.Enter(_lock);
                    _exception = value;
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public static string GoogleApiKey { get { return "AIzaSyBsQBJgLjnLDW_NRil1KcdZC2-iED969hY"; } }
        public static string MapQuestApiKey { get { return "Fmjtd%7Cluu829a2n9%2Ca0%3Do5-947gg4"; } }
    }
}
