using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Streed.DataAccess
{
    public sealed class ActivityRepository
    {
        public Strava.Activities.Activity[] GetActivities()
        {
            var activities = new List<Strava.Activities.Activity>();
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("activities"))
                {
                    foreach (var activityId in userStore.GetDirectoryNames(Path.Combine("activities", "*")))
                    {
                        var filename = string.Format("activities/{0}/activity.json", activityId);
                        if (userStore.FileExists(filename))
                        {
                            using (var stream = userStore.OpenFile(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                var serializer = new DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                                activities.Add((Strava.Activities.Activity)serializer.ReadObject(stream));
                            }
                        }
                    }
                }
            }
            return activities.ToArray();
        }

        public void InsertActivity(Strava.Activities.Activity activity, DataAccess.AthleteRepository athleteRepository = null)
        {
            var existingActivity = GetActivity(activity.Id);
            if (existingActivity != null)
                throw new InvalidOperationException(string.Format("Unable to insert Activity, already exists. {0}({1})", existingActivity.Name, existingActivity.Id));

            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("activities/{0}", activity.Id));
                var filename = string.Format("activities/{0}/activity.json", activity.Id);

                using (var file = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    using (var ms = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                        serializer.WriteObject(ms, activity);
                        ms.Position = 0;
                        var bytes = ms.ToArray();
                        file.Write(bytes, 0, bytes.Length);
                    }
                }

                if (athleteRepository == null) return;

                athleteRepository.InsertAthlete(activity.Athlete);
            }
        }

        public Strava.Activities.Activity GetActivity(long activityId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("activities/{0}/activity.json", activityId);
                if (userStore.FileExists(filename))
                {
                    using (var stream = userStore.OpenFile(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                        return (Strava.Activities.Activity)serializer.ReadObject(stream);
                    }
                }
            }
            return null;
        }

        public bool DoesActivityExist(long activityId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("activities/{0}/activity.json", activityId);
                return (userStore.FileExists(filename));
            }
        }

        public void DeleteActivitiesOlderThan(int days)
        {
            var oldActivities = GetActivities().Where(w => (DateTime.UtcNow.ToUniversalTime() - w.StartDateTime).TotalDays > days).ToList();
            if (oldActivities.Count == 0)
                return;

            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                oldActivities.ForEach(a =>
                    {
                        var filename = string.Format("activities/{0}/activity.json", a.Id);
                        var folder = string.Format("activities/{0}", a.Id);
                        if (userStore.FileExists(filename))
                        {
                            userStore.DeleteFile(filename);
                            userStore.DeleteDirectory(folder);
                        }
                    });
            }
        }

        public void DeleteAllActivities(DataAccess.AthleteRepository athleteRepository = null)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("activities"))
                {
                    var activityIds = userStore.GetDirectoryNames(Path.Combine("activities", "*"));
                    activityIds.ToList().ForEach(activityId =>
                        {
                            var filename = string.Format("activities/{0}/activity.json", activityId);
                            var folder = string.Format("activities/{0}", activityId);
                            if (userStore.FileExists(filename))
                            {
                                userStore.DeleteFile(filename);
                                userStore.DeleteDirectory(folder);
                            }
                        });
                }
            }

            if (athleteRepository == null) return;
            athleteRepository.DeleteAllAthletes();
        }

        public void DeleteActivitiesExcept(IEnumerable<Strava.Activities.Activity> activitiesToKeep)
        { 
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("activities"))
                {
                    var activityIdsInCache = userStore.GetDirectoryNames(Path.Combine("activities", "*"));
                    activityIdsInCache.ToList().ForEach(activityId =>
                        {
                            var id = long.Parse(activityId);
                            if (activitiesToKeep.Where(w => w.Id == id).Any())
                                return;

                            var filename = string.Format("activities/{0}/activity.json", activityId);
                            var folder = string.Format("activities/{0}", activityId);
                            if (userStore.FileExists(filename))
                            {
                                var a = GetActivity(long.Parse(activityId));
                                userStore.DeleteFile(filename);
                                userStore.DeleteDirectory(folder);
                            }
                        });
                }
            }
        }

        public void UpdateActivityKudosCount(long activityId, int kudosCount)
        { 
            var existingActivity = GetActivity(activityId);
            if (existingActivity != null)
            {
                existingActivity.KudosCount = kudosCount;
                existingActivity.HasKudoed = true;
                UpdateActivity(existingActivity);
            }
        }

        public void UpdateActivityCommentCount(long activityId, int commentCount)
        {
            var existingActivity = GetActivity(activityId);
            if (existingActivity != null)
            {
                existingActivity.CommentCount = commentCount;
                UpdateActivity(existingActivity);
            }
        }

        public void UpdateNameDescriptionIsPrivate(long activityId, string name, string description, bool isPrivate)
        {
            var existingActivity = GetActivity(activityId);
            if (existingActivity != null)
            {
                existingActivity.Name = name;
                existingActivity.Description = description;
                existingActivity.Private = isPrivate;
                UpdateActivity(existingActivity);
            }
        }

        public void UpdateActivity(Strava.Activities.Activity activity, DataAccess.AthleteRepository athleteRepository = null)
        {
            var existingActivity = GetActivity(activity.Id);
            if (existingActivity == null)
                throw new KeyNotFoundException(string.Format("Unable to update Activity, does not exist. {0}({1})", activity.Name, activity.Id));

            if (existingActivity.AchievementCount != activity.AchievementCount)
                existingActivity.AchievementCount = activity.AchievementCount;

            if (existingActivity.ActivityZones == null && activity.ActivityZones != null)
                existingActivity.ActivityZones = activity.ActivityZones;

            if (existingActivity.AltitudeStream == null && activity.AltitudeStream != null)
                existingActivity.AltitudeStream = activity.AltitudeStream;

            if (existingActivity.Athlete == null && activity.Athlete != null)
                existingActivity.Athlete = activity.Athlete;

            if (existingActivity.AthleteCount != activity.AthleteCount)
                existingActivity.AthleteCount = activity.AthleteCount;

            if (existingActivity.AverageCadence != activity.AverageCadence)
                existingActivity.AverageCadence = activity.AverageCadence;

            if (existingActivity.AverageHeartRate != activity.AverageHeartRate)
                existingActivity.AverageHeartRate = activity.AverageHeartRate;

            if (existingActivity.AverageSpeedInMetersPerSecond != activity.AverageSpeedInMetersPerSecond)
                existingActivity.AverageSpeedInMetersPerSecond = activity.AverageSpeedInMetersPerSecond;

            if (existingActivity.AverageTempInCelsius != activity.AverageTempInCelsius)
                existingActivity.AverageTempInCelsius = activity.AverageTempInCelsius;

            if (existingActivity.AverageWatts != activity.AverageWatts)
                existingActivity.AverageWatts = activity.AverageWatts;

            if (existingActivity.Calories != activity.Calories)
                existingActivity.Calories = activity.Calories;

            if (existingActivity.CommentCount != activity.CommentCount)
                existingActivity.CommentCount = activity.CommentCount;

            if (existingActivity.Description != activity.Description)
                existingActivity.Description = activity.Description;

            if (existingActivity.DistanceInMeters != activity.DistanceInMeters)
                existingActivity.DistanceInMeters = activity.DistanceInMeters;

            if (existingActivity.DistanceStream == null && activity.DistanceStream != null)
                existingActivity.DistanceStream = activity.DistanceStream;

            if (existingActivity.ElapsedTimeInSeconds != activity.ElapsedTimeInSeconds)
                existingActivity.ElapsedTimeInSeconds = activity.ElapsedTimeInSeconds;

            if (existingActivity.GradientStream == null && activity.GradientStream != null)
                existingActivity.GradientStream = activity.GradientStream;

            if (existingActivity.HasKudoed != activity.HasKudoed)
                existingActivity.HasKudoed = activity.HasKudoed;

            if (existingActivity.HeartRateStream == null && activity.HeartRateStream != null)
                existingActivity.HeartRateStream = activity.HeartRateStream;

            if (existingActivity.IsDeviceWatts != activity.IsDeviceWatts)
                existingActivity.IsDeviceWatts = activity.IsDeviceWatts;

            if (existingActivity.Kilojoules != activity.Kilojoules)
                existingActivity.Kilojoules = activity.Kilojoules;

            if (existingActivity.KudosCount != activity.KudosCount)
                existingActivity.KudosCount = activity.KudosCount;

            if (existingActivity.Laps == null && activity.Laps != null)
                existingActivity.Laps = activity.Laps;

            if (existingActivity.Map == null && activity.Map != null)
                existingActivity.Map = activity.Map;

            if (existingActivity.MaxHeartRate != activity.MaxHeartRate)
                existingActivity.MaxHeartRate = activity.MaxHeartRate;

            if (existingActivity.MaxSpeedInMetersPerSecond != activity.MaxSpeedInMetersPerSecond)
                existingActivity.MaxSpeedInMetersPerSecond = activity.MaxSpeedInMetersPerSecond;

            if (existingActivity.MovingTimeInSeconds != activity.MovingTimeInSeconds)
                existingActivity.MovingTimeInSeconds = activity.MovingTimeInSeconds;

            if (existingActivity.Name != activity.Name)
                existingActivity.Name = activity.Name;

            if (existingActivity.Private != activity.Private)
                existingActivity.Private = activity.Private;

            if (existingActivity.ResourceState < activity.ResourceState)
                existingActivity.ResourceState = activity.ResourceState;

            if (existingActivity.SegmentEfforts == null && activity.SegmentEfforts != null)
                existingActivity.SegmentEfforts = activity.SegmentEfforts;

            if (existingActivity.StartDate != activity.StartDate)
                existingActivity.StartDate = activity.StartDate;

            if (existingActivity.StartDateLocal != activity.StartDateLocal)
                existingActivity.StartDateLocal = activity.StartDateLocal;

            if (existingActivity.StartDateTimezone != activity.StartDateTimezone)
                existingActivity.StartDateTimezone = activity.StartDateTimezone;

            if (existingActivity.TimeStream == null && activity.TimeStream != null)
                existingActivity.TimeStream = activity.TimeStream;

            if (existingActivity.TotalElevationGainInMeters != activity.TotalElevationGainInMeters)
                existingActivity.TotalElevationGainInMeters = activity.TotalElevationGainInMeters;

            if (existingActivity.Truncated != activity.Truncated)
                existingActivity.Truncated = activity.Truncated;

            if (existingActivity.TypeString != activity.TypeString)
                existingActivity.TypeString = activity.TypeString;

            if (existingActivity.WattsStream == null && activity.WattsStream != null)
                existingActivity.WattsStream = activity.WattsStream;

            if (existingActivity.WeightedAverageWatts != activity.WeightedAverageWatts)
                existingActivity.WeightedAverageWatts = activity.WeightedAverageWatts;
            
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("activities/{0}/activity.json", existingActivity.Id);
                using (var file = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    using (var ms = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Strava.Activities.Activity));
                        serializer.WriteObject(ms, existingActivity);
                        ms.Position = 0;
                        var bytes = ms.ToArray();
                        file.Write(bytes, 0, bytes.Length);
                    }
                }

                if (athleteRepository == null) return;
                athleteRepository.InsertAthlete(existingActivity.Athlete);
            }
        }
    }
}
