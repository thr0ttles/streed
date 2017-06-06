using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.ViewModels
{
    public sealed class ActivityViewModel : BaseViewModel
    {
        #region Properties
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetProperty<string>(ref _type, value);
            }
        }

        public Strava.Activities.Activity Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                SetProperty<Strava.Activities.Activity>(ref _activity, value);
            }
        }

        public Strava.Activities.ActivityZone HeartRateActivityZone
        {
            get
            {
                return _heartRateActivityZone;
            }
            set
            {
                SetProperty<Strava.Activities.ActivityZone>(ref _heartRateActivityZone, value);
            }
        }

        public Strava.Activities.ActivityZone PowerActivityZone
        {
            get
            {
                return _powerActivityZone;
            }
            set
            {
                SetProperty<Strava.Activities.ActivityZone>(ref _powerActivityZone, value);
            }
        }

        public ObservableCollection<Models.HeartRateZoneData> HeartRateData
        {
            get
            {
                return _heartRateData;
            }
            set
            {
                SetProperty<ObservableCollection<Models.HeartRateZoneData>>(ref _heartRateData, value);
            }
        }

        public ObservableCollection<Models.PowerDistributionData> PowerData
        {
            get
            {
                return _powerData;
            }
            set
            {
                SetProperty<ObservableCollection<Models.PowerDistributionData>>(ref _powerData, value);
            }
        }

        public ObservableCollection<Models.PowerZoneData> PowerZoneData
        {
            get
            {
                return _powerZoneData;
            }
            set
            {
                SetProperty<ObservableCollection<Models.PowerZoneData>>(ref _powerZoneData, value);
            }
        }

        public ObservableCollection<Strava.Segments.SegmentEffort> SegmentEfforts
        {
            get
            {
                return _segmentEfforts;
            }
            set
            {
                SetProperty<ObservableCollection<Strava.Segments.SegmentEffort>>(ref _segmentEfforts, value);
            }
        }

        public bool IsLoaded { get { return _isLoaded; } set { SetProperty<bool>(ref _isLoaded, value); } }

        public ObservableCollection<Strava.Activities.Lap> Laps
        {
            get
            {
                return _laps;
            }
            set
            {
                SetProperty<ObservableCollection<Strava.Activities.Lap>>(ref _laps, value);
            }
        }

        public Strava.Activities.Lap CurrentLap
        {
            get
            {
                return _currentLap;
            }
            set
            {
                if (SetProperty<Strava.Activities.Lap>(ref _currentLap, value))
                {
                    if (value == null)
                    {
                        CurrentLap = _specialOverallLap;
                    }
                }
            }
        }

        #endregion

        public async Task<bool> Load(long activityId)
        {
            var repo = new DataAccess.ActivityRepository();
            Activity = repo.GetActivity(activityId);

            var loadFromApi = false;
            if (Activity == null) loadFromApi = true;
            else loadFromApi = (Activity.ResourceState < 3);

            #region load from cache
            if (loadFromApi == false)
            {
                Activity.Athlete.LoadProfileImages();
                Activity.Map.LoadSummaryMaps();
                SegmentEfforts = new ObservableCollection<Strava.Segments.SegmentEffort>(Activity.SegmentEfforts);

                if (Activity.Athlete.ResourceState < 3)
                {
                    var athlete = new DataAccess.AthleteRepository().GetAthlete(Activity.Athlete.Id);
                    if (athlete.ResourceState < 3)
                    {
                        athlete = await Strava.Api.Athletes.GetAthlete(athlete.Id, DataAccess.StreedApplicationSettings.AccessToken.Token);
                        new DataAccess.AthleteRepository().InsertAthlete(athlete);
                    }
                    Activity.Athlete = athlete;
                }
            }
            #endregion

            #region load from api
            if (loadFromApi)
            {
                Activity = await Strava.Api.Activities.GetActivity(activityId, DataAccess.StreedApplicationSettings.AccessToken.Token);
                if (Activity == null) return false;

                Activity.Athlete.LoadProfileImages();
                Activity.Map.LoadSummaryMaps();

                SegmentEfforts = new ObservableCollection<Strava.Segments.SegmentEffort>(Activity.SegmentEfforts);
            }
            #endregion

            #region heartrate and power activity zones
            if (loadFromApi == true && Activity.ActivityZones == null)
            {
                var activityZones = await Strava.Api.Activities.GetZones(activityId, DataAccess.StreedApplicationSettings.AccessToken.Token);
                if (activityZones != null && activityZones.Count() != 0)
                {
                    var heartRateActivityZone = activityZones.Where(w => w.Type == "heartrate").SingleOrDefault();
                    if (heartRateActivityZone != null && heartRateActivityZone.DistributionBuckets != null && heartRateActivityZone.DistributionBuckets.Any())
                    {
                        //if the data adds up to zero, just remove the data altogether
                        if (heartRateActivityZone.DistributionBuckets.Sum(s => s.Time) == 0)
                            heartRateActivityZone.DistributionBuckets = null;
                    }

                    var powerActivityZone = activityZones.Where(w => w.Type == "power").SingleOrDefault();
                    if (powerActivityZone != null && powerActivityZone.DistributionBuckets != null && powerActivityZone.DistributionBuckets.Any())
                    {
                        //if the data adds up to zero, just remove the data altogether
                        if (powerActivityZone.DistributionBuckets.Sum(s => s.Time) == 0)
                            powerActivityZone.DistributionBuckets = null;
                    }

                    Activity.ActivityZones = activityZones;
                }
            }

            if (Activity.ActivityZones != null && Activity.ActivityZones.Count() != 0)
            {
                HeartRateActivityZone = Activity.ActivityZones.Where(w => w.Type == "heartrate").SingleOrDefault();
                if (HeartRateActivityZone != null)
                {
                    var hrd = Models.HeartRateZoneData.DistributionBucketsToHeartRateData(HeartRateActivityZone.DistributionBuckets,
                        Activity.MovingTimeInSeconds, HeartRateActivityZone.Max);
                    HeartRateData = new ObservableCollection<Models.HeartRateZoneData>(hrd);
                }

                PowerActivityZone = Activity.ActivityZones.Where(w => w.Type == "power").SingleOrDefault();
            }
            #endregion

            #region load streams
            if (loadFromApi == true &&
                Activity.WattsStream == null &&
                Activity.HeartRateStream == null &&
                Activity.DistanceStream == null)
            {
                var streams = await Strava.Api.Streams.GetAllStreamsExcludingAltitude(activityId, DataAccess.StreedApplicationSettings.AccessToken.Token);
                if (streams != null)
                {
                    if (streams.Any(w => w.Type == "watts"))
                        Activity.WattsStream = streams.Where(w => w.Type == "watts").SingleOrDefault();
                    else if (streams.Any(w => w.Type == "watts_calc"))
                        Activity.WattsStream = streams.Where(w => w.Type == "watts_calc").SingleOrDefault();

                    Activity.HeartRateStream = streams.Where(w => w.Type == "heartrate").SingleOrDefault();
                    Activity.DistanceStream = streams.Where(w => w.Type == "distance").SingleOrDefault();
                }
            }
            #endregion

            #region watts
            if (Activity.WattsStream != null)
            {
                if (Activity.Athlete.ResourceState < 3)
                {
                    var athlete = new DataAccess.AthleteRepository().GetAthlete(Activity.Athlete.Id);
                    if (athlete.ResourceState < 3)
                    {
                        athlete = await Strava.Api.Athletes.GetAthlete(athlete.Id, DataAccess.StreedApplicationSettings.AccessToken.Token);
                        new DataAccess.AthleteRepository().InsertAthlete(athlete);
                    }
                    Activity.Athlete = athlete;
                }

                var pdd = Models.PowerDistributionData.StreamToPowerDistributionData(Activity.WattsStream);
                PowerData = new ObservableCollection<Models.PowerDistributionData>(pdd);

                var pzd = Models.PowerZoneData.StreamToPowerZoneData(Activity.WattsStream, Activity.Athlete.Ftp);
                PowerZoneData = new ObservableCollection<Models.PowerZoneData>(pzd);
            }
            else
            {
                PowerData = new ObservableCollection<Models.PowerDistributionData>();
                PowerZoneData = new ObservableCollection<Models.PowerZoneData>();
            }
            #endregion

            #region laps (uses heartrate and watts)
            if (loadFromApi == true || 
                Activity.Laps == null)
            {
                Activity.Laps = await Strava.Api.Activities.GetLaps(activityId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                if (Activity.Laps != null && Activity.WattsStream != null && Activity.WattsStream.Data != null)
                {
                    var calcDecoupling = (Activity.HeartRateStream != null && Activity.HeartRateStream.Data != null &&
                                          Activity.WattsStream.Data.Length == Activity.HeartRateStream.Data.Length);

                    var onlyOneLap = Activity.Laps.Count() == 1;

                    Activity.Laps.ToList().ForEach(lap =>
                        {
                            lap.IF = Strava.Utilities.CalculateIntensityFactor(lap.AverageWatts, Activity.Athlete.Ftp);

                            var activityWatts = Activity.WattsStream.Data;
                            object[] activityHeartRate = null;
                            if (calcDecoupling) 
                                activityHeartRate = Activity.HeartRateStream.Data;

                            //fix-up EndIndex, sometimes its set to 0
                            if (onlyOneLap && lap.EndIndex == 0)
                                lap.EndIndex = activityWatts.Length - 1;

                            if (lap.EndIndex > lap.StartIndex &&
                                lap.StartIndex <= activityWatts.Length && lap.EndIndex <= activityWatts.Length)
                            {
                                var range = lap.EndIndex - lap.StartIndex + 1;
                                var lapWatts = new double[range];
                                int[] lapHeartRate = null;
                                if (calcDecoupling)
                                    lapHeartRate = new int[range];

                                for (var i = 0; i < range; i++)
                                {
                                    lapWatts[i] = Convert.ToDouble(activityWatts[i + lap.StartIndex]);
                                    if (calcDecoupling)
                                        lapHeartRate[i] = Convert.ToInt32(activityHeartRate[i + lap.StartIndex]);
                                }

                                var lapNormalizedWatts = Strava.Utilities.CalculateNormalizedWatts(lapWatts);
                                lap.WeightedAverageWatts = lapNormalizedWatts;
                                lap.VI = Strava.Utilities.CalculateVariabilityIndex(lapNormalizedWatts, lap.AverageWatts);
                                lap.EF = Strava.Utilities.CalculateEfficiencyFactor(lapNormalizedWatts, lap.AverageHeartRate);
                                lap.TSS = Strava.Utilities.CalculateTSS(lap.MovingTimeInSeconds, lapNormalizedWatts, lap.IF, Activity.Athlete.Ftp);

                                if (calcDecoupling)
                                    lap.PwHr = Strava.Utilities.CalculateDecoupling(lapWatts, lapHeartRate);
                            }
                        });
                }
            }

            if (Laps == null)
            {
                Laps = new ObservableCollection<Strava.Activities.Lap>(Activity.Laps);
            }
            else
            {
                Laps.Clear();
                Activity.Laps.ToList().ForEach(l => Laps.Add(l));
            }

            if (Activity.Laps != null && Activity.Laps.Count() == 1)
            {
                CurrentLap = Activity.Laps.Single();
            }
            else
            {
                _specialOverallLap = new Strava.Activities.Lap 
                    { 
                        Name = "Overall",
                        Activity = Activity,
                        Athlete = Activity.Athlete,
                        AverageCadence = Activity.AverageCadence,
                        AverageHeartRate = Activity.AverageHeartRate,
                        AverageSpeedInMetersPerSecond = Activity.AverageSpeedInMetersPerSecond,
                        AverageWatts = Activity.AverageWatts,
                        DeviceWatts = Activity.IsDeviceWatts,
                        DistanceInMeters = Activity.DistanceInMeters,
                        ElapsedTimeInSeconds = Activity.ElapsedTimeInSeconds,
                        MaxHeartRate = Activity.MaxHeartRate,
                        MaxSpeedInMetersPerSecond = Activity.MaxSpeedInMetersPerSecond,
                        MovingTimeInSeconds = Activity.MovingTimeInSeconds,
                        StartDate = Activity.StartDate,
                        StartDateLocal = Activity.StartDateLocal,
                        TotalElevationGainInMeters = Activity.TotalElevationGainInMeters,
                        WeightedAverageWatts = Activity.WeightedAverageWatts
                    };

                _specialOverallLap.IF = Strava.Utilities.CalculateIntensityFactor(_specialOverallLap.AverageWatts, Activity.Athlete.Ftp);

                if (Activity.WattsStream != null && Activity.WattsStream.Data != null)
                {
                    var calcDecoupling = (Activity.HeartRateStream != null && Activity.HeartRateStream.Data != null &&
                                          Activity.WattsStream.Data.Length == Activity.HeartRateStream.Data.Length);

                    var activityWatts = Activity.WattsStream.Data;
                    object[] activityHeartRate = null;
                    if (calcDecoupling) 
                        activityHeartRate = Activity.HeartRateStream.Data;

                    var len = Activity.WattsStream.Data.Length;
                    var wattsData = new double[len];
                    int[] heartRateData = null;
                    if (calcDecoupling)
                        heartRateData = new int[len];

                    for (var i = 0; i < len; i++)
                    {
                        wattsData[i] = Convert.ToDouble(Activity.WattsStream.Data[i]);
                        if (calcDecoupling)
                            heartRateData[i] = Convert.ToInt32(Activity.HeartRateStream.Data[i]);
                    }

                    _specialOverallLap.WeightedAverageWatts = Activity.WeightedAverageWatts;
                    _specialOverallLap.VI = Strava.Utilities.CalculateVariabilityIndex(_specialOverallLap.WeightedAverageWatts, Activity.AverageWatts);
                    _specialOverallLap.EF = Strava.Utilities.CalculateEfficiencyFactor(_specialOverallLap.WeightedAverageWatts, Activity.AverageHeartRate);
                    _specialOverallLap.TSS = Strava.Utilities.CalculateTSS(Activity.MovingTimeInSeconds, _specialOverallLap.WeightedAverageWatts, _specialOverallLap.IF, Activity.Athlete.Ftp);

                    if (calcDecoupling)
                        _specialOverallLap.PwHr = Strava.Utilities.CalculateDecoupling(wattsData, heartRateData);
                }

                CurrentLap = _specialOverallLap;
            }
            #endregion

            //save the activity to the cache, some data may have been updated
            new DataAccess.ActivityRepository().UpdateActivity(Activity);

            IsLoaded = true;
            return true;
        }

        public void Refresh()
        {
            if (Activity == null)
                return;

            var repo = new DataAccess.ActivityRepository();
            Activity = repo.GetActivity(Activity.Id);

            return;
        }

        private string _type = string.Empty;
        private Strava.Activities.Activity _activity = null;
        private ObservableCollection<Strava.Segments.SegmentEffort> _segmentEfforts = null;
        private Strava.Activities.ActivityZone _heartRateActivityZone = null;
        private Strava.Activities.ActivityZone _powerActivityZone = null;
        private ObservableCollection<Models.HeartRateZoneData> _heartRateData = null;
        private ObservableCollection<Models.PowerDistributionData> _powerData = null;
        private ObservableCollection<Models.PowerZoneData> _powerZoneData = null;
        private bool _isLoaded = false;
        private ObservableCollection<Strava.Activities.Lap> _laps = null;
        private Strava.Activities.Lap _currentLap = null;
        private Strava.Activities.Lap _specialOverallLap = null;
    }
}
