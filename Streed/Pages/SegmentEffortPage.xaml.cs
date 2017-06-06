using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.Text;
using System.Collections.ObjectModel;

namespace Streed.Pages
{
    public partial class SegmentEffortPage : PhoneApplicationPage
    {
        private long SegmentEffortId { get; set; }
        private Strava.Segments.SegmentEffort SegmentEffort { get; set; }
        private ObservableCollection<Strava.Segments.LeaderboardEffort> LeaderboardEfforts { get; set; }
        private bool IsLeaderboardOverall { get; set; }
        private List<Strava.Segments.LeaderboardEffort> OverallLeaderboardEfforts { get; set; }
        private List<Strava.Segments.LeaderboardEffort> ThisYearLeaderboardEfforts { get; set; }
        private bool HasLoadedOverall { get; set; }
        private bool HasLoadedThisYear { get; set; }
        private bool IsLoaded { get; set; }

        public SegmentEffortPage()
        {
            InitializeComponent();

            IsLoaded = false;
            OverallLeaderboardEfforts = new List<Strava.Segments.LeaderboardEffort>();
            ThisYearLeaderboardEfforts = new List<Strava.Segments.LeaderboardEffort>();
            HasLoadedThisYear = false;
            HasLoadedOverall = false;

            LeaderboardEfforts = new ObservableCollection<Strava.Segments.LeaderboardEffort>();
            LeaderboardList.ItemsSource = LeaderboardEfforts;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (IsLoaded)
                return;

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                SegmentEffortId = long.Parse(id);
            }

            try
            {
                Progress.IsRunning = true;
                ContentPanel.IsHitTestVisible = false;
                ContentPanel.Opacity = .25;

                await LoadSegmentEffort();

                if (SegmentEffort != null && SegmentEffort.Athlete != null && SegmentEffort.Athlete.ResourceState < 2)
                {
                    var athleteRepo = new DataAccess.AthleteRepository();
                    var athlete = athleteRepo.GetAthlete(SegmentEffort.Athlete.Id);
                    var loadAthleteFromApi = (athlete != null && athlete.ResourceState < 2);
                    if (loadAthleteFromApi || athlete == null)
                    {
                        athlete = await Strava.Api.Athletes.GetAthlete(SegmentEffort.Athlete.Id, DataAccess.StreedApplicationSettings.AccessToken.Token);
                        if (athlete != null)
                            athleteRepo.InsertAthlete(athlete);
                    }
                    SegmentEffort.Athlete = athlete;
                }

                if (SegmentEffort != null && SegmentEffort.Activity != null && SegmentEffort.Activity.ResourceState < 2)
                {
                    var activityRepo = new DataAccess.ActivityRepository();
                    var exisitingActivity = activityRepo.GetActivity(SegmentEffort.Activity.Id);
                    var loadActivityFromApi = (exisitingActivity != null && exisitingActivity.ResourceState < 2);
                    if (loadActivityFromApi || exisitingActivity == null)
                    {
                        var activity = await Strava.Api.Activities.GetActivity(SegmentEffort.Activity.Id, DataAccess.StreedApplicationSettings.AccessToken.Token);
                        if (activity != null)
                        {
                            if (exisitingActivity == null)
                            {
                                activityRepo.InsertActivity(activity);
                            }
                            else
                            {
                                activityRepo.UpdateActivity(activity);
                            }

                            SegmentEffort.Activity = activity;
                        }
                    }
                    else
                    {
                        SegmentEffort.Activity = exisitingActivity;
                    }
                }

                IsLeaderboardOverall = RadioButtonOverall.IsChecked.Value;

                if (SegmentEffort != null && SegmentEffort.Segment != null)
                {
                    await LoadLeaderboardThisYear();
                    await LoadLeaderboardOverall();
                }

                if (IsLeaderboardOverall) OverallLeaderboardEfforts.ForEach(effort => LeaderboardEfforts.Add(effort));
                else ThisYearLeaderboardEfforts.ForEach(effort => LeaderboardEfforts.Add(effort));

                SetBestEffortAndRank();

                if (SegmentEffort != null && SegmentEffort.YearlyBestEffort == "-")
                { 
                    //no yearly efforts, default to overall
                    ThisYearsBestEffort.Visibility = System.Windows.Visibility.Collapsed;
                    ThisYearsRank.Visibility = System.Windows.Visibility.Collapsed;

                    LeaderboardEfforts.Clear();
                    OverallLeaderboardEfforts.ForEach(effort => LeaderboardEfforts.Add(effort));
                    RadioButtonOverall.IsChecked = true;
                    IsLeaderboardOverall = true;
                }

                DataContext = SegmentEffort;
                IsLoaded = true;
            }
            finally
            {
                Progress.IsRunning = false;
                ContentPanel.IsHitTestVisible = true;
                ContentPanel.Opacity = 1;
            }
        }

        private async Task LoadSegmentEffort()
        {
            try
            {
                SegmentEffort = await Strava.Api.Segments.GetSegmentEffort(SegmentEffortId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                if (SegmentEffort == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                await LoadSegment();
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
        }

        private async Task LoadLeaderboardThisYear()
        {
            try
            {
                var leaderboard = await Strava.Api.Segments.GetSegmentLeaderboard(SegmentEffort.Segment.Id, 
                    DataAccess.StreedApplicationSettings.AccessToken.Token, SegmentEffort.Athlete.Sex, true);

                if (leaderboard == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                HasLoadedThisYear = true;
                ThisYearLeaderboardEfforts.AddRange(leaderboard.Entries);

                leaderboard.Entries.ToList().ForEach(e =>
                {
                    //set segment distance in leaderboard effort
                    //for consistent speed calculations
                    e.SegmentDistanceInMeters = SegmentEffort.DistanceInMeters;
                });
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
        }

        private async Task LoadLeaderboardOverall()
        {
            try
            {
                var leaderboard = await Strava.Api.Segments.GetSegmentLeaderboard(SegmentEffort.Segment.Id, 
                    DataAccess.StreedApplicationSettings.AccessToken.Token, SegmentEffort.Athlete.Sex);

                if (leaderboard == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                HasLoadedOverall = true;
                OverallLeaderboardEfforts.AddRange(leaderboard.Entries);

                leaderboard.Entries.ToList().ForEach(e =>
                {
                    //set segment distance in leaderboard effort
                    //for consistent speed calculations
                    e.SegmentDistanceInMeters = SegmentEffort.DistanceInMeters;
                });
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
        }

        private async Task LoadSegment()
        {
            try
            {
                var segmentId = SegmentEffort.Segment.Id;
                var segment = await Strava.Api.Segments.GetSegment(segmentId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                if (segment == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                SegmentEffort.Segment = segment;
                SegmentEffort.Segment.Map.LoadDetailMap();
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
        }

        private void SetBestEffortAndRank()
        {
            if (OverallLeaderboardEfforts.Where(w => w.AthleteId == SegmentEffort.Athlete.Id).Any())
            {
                var overallEffort = OverallLeaderboardEfforts.Where(w => w.AthleteId == SegmentEffort.Athlete.Id).Take(1).Single();
                SegmentEffort.OverallBestEffort = overallEffort.ElapsedTime;
                SegmentEffort.OverallRank = overallEffort.Rank.ToString();
            }
            else
            {
                SegmentEffort.OverallBestEffort = "-";
                SegmentEffort.OverallRank = "-";
            }

            if (ThisYearLeaderboardEfforts.Where(w => w.AthleteId == SegmentEffort.Athlete.Id).Any())
            {
                var yearlyEffort = ThisYearLeaderboardEfforts.Where(w => w.AthleteId == SegmentEffort.Athlete.Id).Take(1).Single();
                SegmentEffort.YearlyBestEffort = yearlyEffort.ElapsedTime;
                SegmentEffort.YearlyRank = yearlyEffort.Rank.ToString();
            }
            else
            {
                SegmentEffort.YearlyBestEffort = "-";
                SegmentEffort.YearlyRank = "-";
            }
        }

        private void RadioButtonThisYear_Click(object sender, RoutedEventArgs e)
        {
            if (IsLeaderboardOverall)
            {
                IsLeaderboardOverall = false;
                LeaderboardEfforts.Clear();
                ThisYearLeaderboardEfforts.ForEach(effort => LeaderboardEfforts.Add(effort));
            }
        }

        private void RadioButtonOverall_Click(object sender, RoutedEventArgs e)
        {
            if (IsLeaderboardOverall == false)
            {
                IsLeaderboardOverall = true;
                LeaderboardEfforts.Clear();
                OverallLeaderboardEfforts.ForEach(effort => LeaderboardEfforts.Add(effort));
            }
        }

        private void BestEffortGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ThisYearsBestEffort.Visibility == System.Windows.Visibility.Visible) BestEffortGridFlipAnimationToBack.Begin();
            else BestEffortGridFlipAnimationToFront.Begin();
        }

        private void RankGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ThisYearsRank.Visibility == System.Windows.Visibility.Visible) RankGridFlipAnimationToBack.Begin();
            else RankGridFlipAnimationToFront.Begin();
        }

        private void LeaderboardSegmentEffortHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var effortId = ((FrameworkElement)sender).Tag;
            NavigationService.Navigate(new Uri(string.Format("/Pages/SegmentEffortPage.xaml?id={0}", effortId), UriKind.Relative));
        }

        private void SegmentEffortActivityHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var activityId = ((FrameworkElement)sender).Tag;
            if (SegmentEffort != null && SegmentEffort.Activity != null &&
                SegmentEffort.Activity.Athlete != null &&
                SegmentEffort.Activity.Athlete.Id == DataAccess.StreedApplicationSettings.AuthenticatedAthleteId)
                NavigationService.Navigate(new Uri(string.Format("/Pages/ActivityPage.xaml?id={0}", activityId), UriKind.Relative));
            else
                NavigationService.Navigate(new Uri(string.Format("/Pages/BasicActivityPage.xaml?id={0}", activityId), UriKind.Relative));
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            if (SegmentEffort != null && SegmentEffort.Segment != null && SegmentEffort.Segment.Map != null)
            {
                string polyline = null;
                if (string.IsNullOrWhiteSpace(SegmentEffort.Segment.Map.Polyline) == false)
                    polyline = SegmentEffort.Segment.Map.Polyline;
                else if (string.IsNullOrWhiteSpace(SegmentEffort.Segment.Map.SummaryPolyline) == false)
                    polyline = SegmentEffort.Segment.Map.SummaryPolyline;

                if (string.IsNullOrWhiteSpace(polyline) == false)
                {
                    polyline = polyline.Replace(@"\", "%5c");
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MapPage.xaml?polyline={0}", polyline), UriKind.Relative));
                }
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}