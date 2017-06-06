using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Streed.Resources;
using System.Text;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Phone.Info;

namespace Streed
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool IsAuthCompleted { get; set; }
        private long LastActivityId = 0;
        private int CurrentPage { get; set; }
        private bool IsCurrentlyLoading { get; set; }
        private bool IsMoreToLoad { get; set; }
        private readonly int ActivitiesPerPage = 20;
        private Strava.Activities.ActivityObservableCollection Activities { get; set; }
        private ObservableCollection<Strava.Activities.GroupedActivityObservableCollection> GroupedActivities { get; set; }
        private bool IsCurrentlyHandlingKudosOrComment { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            IsAuthCompleted = false;
            Activities = new Strava.Activities.ActivityObservableCollection();
            GroupedActivities = new ObservableCollection<Strava.Activities.GroupedActivityObservableCollection>();

            Feed.ItemsSource = GroupedActivities;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected override Size MeasureOverride(Size availableSize)
        {
            var main = Application.Current.RootVisual as PhoneApplicationFrame;
            if (main != null)
            {
                Browser.Width = App.Current.Host.Content.ActualWidth;
                Browser.Height = App.Current.Host.Content.ActualHeight;

                FilterListBox.Width = App.Current.Host.Content.ActualWidth;
                FilterListBox.Height = App.Current.Host.Content.ActualHeight;
            }
            return base.MeasureOverride(availableSize);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Reset)
                return;

            //remove all backstack entries so the user can't go back
            //in case we came back to this page because we need to re-auth
            while (App.RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }

            if (DataAccess.StreedApplicationSettings.HasAccessToken == false)
            {
                Activities.Clear();
                GroupedActivities.Clear();
                BeginStravaAuthorization();
                return;
            }

            if (State.ContainsKey("LastActivityId"))
            {
                LastActivityId = (long)State["LastActivityId"];
            }

            FilterText.Text = DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends ? "Following" : "Me";

            if (LastActivityId == 0 && 
                DataAccess.StreedApplicationSettings.ShouldRefreshFeed)
            {
                CurrentPage = 1;
                Activities.Clear();
                GroupedActivities.Clear();
                LastActivityId = 0;

                await LoadFeed();

                if (Activities.Any())
                {
                    var activityRepo = new DataAccess.ActivityRepository();
                    activityRepo.DeleteActivitiesExcept(Activities);

                    var repo = new DataAccess.MapRepository();
                    var days = 7;
                    repo.DeleteMapsLastAccessedInDays(days);
                }
            }

            if (Activities.Any() == false)
            {
                await LoadFeedFromRepository();
            }

            if (LastActivityId != 0 && e.NavigationMode == NavigationMode.Back)
            {
                UpdateLastActivity();
                LastActivityId = 0;
            }

            LastUpdated.Text = string.Format("LAST UPDATED {0}", DataAccess.StreedApplicationSettings.RefreshedFeedAgo.ToUpper());
            EmptyState.Visibility = Activities.Any() ? Visibility.Collapsed : Visibility.Visible;

            var rateReminder = new Telerik.Windows.Controls.RadRateApplicationReminder();
            if (rateReminder.AreFurtherRemindersSkipped == false)
            {
                rateReminder.AllowUsersToSkipFurtherReminders = true;
                rateReminder.RecurrencePerUsageCount = 20;
                rateReminder.SkipFurtherRemindersOnYesPressed = true;
                rateReminder.MessageBoxInfo.Title = "Rate Streed!";
                rateReminder.MessageBoxInfo.Content = "I made Streed to bring a great Strava feed experience to Windows Phone users like you and me. Please rate my app, I'd love to hear your feedback.";
                rateReminder.Notify();
            }

            try
            {
                //always update the current (authenticated) athlete
                //ftp and other values may have changed which will affect metrics, etc.
                var currentAthlete = await Strava.Api.Athletes.GetCurrentAthlete(DataAccess.StreedApplicationSettings.AccessToken.Token);
                if (currentAthlete != null) new DataAccess.AthleteRepository().InsertAthlete(currentAthlete);
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                BeginStravaAuthorization();
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
 	         base.OnNavigatedFrom(e);

             State["LastActivityId"] = LastActivityId;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (IsAuthCompleted == false)
            {
                if (Browser.CanGoBack)
                {
                    Browser.GoBack();
                    e.Cancel = true;
                }
            }

            base.OnBackKeyPress(e);
        }

        private async void BeginStravaAuthorization()
        {
            ApplicationBar.IsVisible = false;
            IsAuthCompleted = false;
            AuthorizationPopup.Visibility = System.Windows.Visibility.Visible;
            AuthorizationPopup.IsOpen = true;
            DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends = true;

            string width = "480";
            string height = "853";
            var uri = new Uri(String.Format("default.html#width={0}&height={1}", width, height), UriKind.Relative);

            //clear browser cookies so the Strava/Facebook auth sessions are removed
            //whenever we are navigating to the default page becuase this is the
            //beginning of the auth process.
            await Browser.ClearCookiesAsync();

            Browser.Navigated += Browser_Navigated;
            Browser.Navigating += Browser_Navigating;
            Browser.NavigationFailed += Browser_NavigationFailed;
            Browser.IsScriptEnabled = true;
            Browser.Navigate(uri);
        }

        private void Browser_Navigated(object sender, NavigationEventArgs e)
        {
            BrowserProgress.IsRunning = false;
        }

        private void Browser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            BrowserProgress.IsRunning = false;

            if (IsAuthCompleted == false)
            {
                e.Handled = true;
                PopupErrorBanner.Opacity = 1;
                PopupErrorBanner.Visibility = System.Windows.Visibility.Visible;
                PopupErrorBannerFadeAnimation.Begin();
                BeginStravaAuthorization();
            }
        }

        private async void Browser_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.OriginalString.Contains("default.html"))
                return;

            if (e.Uri.OriginalString == "https://www.strava.com" || e.Uri.OriginalString == "https://www.strava.com/dashboard")
            {
                e.Cancel = true;
                BeginStravaAuthorization();
                return;
            }

            if (e.Uri.Host == "localhost")
            {
                e.Cancel = true;
                ApplicationBar.IsVisible = true;
                IsAuthCompleted = true;
                AuthorizationPopup.Visibility = System.Windows.Visibility.Collapsed;
                AuthorizationPopup.IsOpen = false;

                var codeQueryIndex = e.Uri.Query.IndexOf("code=");
                if (codeQueryIndex != -1)
                {
                    var codeStartIndex = codeQueryIndex + 5;
                    var code = e.Uri.Query.Substring(codeStartIndex);
                    var additionalQueryIndex = code.IndexOf('&');
                    if (additionalQueryIndex != -1)
                    {
                        code = code.Substring(0, additionalQueryIndex);
                    }

                    Progress.Content = "Loading...";
                    Progress.IsRunning = true;

                    DataAccess.StreedApplicationSettings.AccessToken =
                        await Strava.Api.Authentication.Authorize(DataAccess.StreedApplicationSettings.ClientId, DataAccess.StreedApplicationSettings.ClientSecret, code);

                    if (DataAccess.StreedApplicationSettings.HasAccessToken == false)
                    {
                        BeginStravaAuthorization();
                        return;
                    }

                    new DataAccess.AthleteRepository().InsertAthlete(DataAccess.StreedApplicationSettings.AccessToken.Athlete);

                    LastActivityId = 0;
                    CurrentPage = 1;
                    LastUpdated.Text = string.Empty;
                    Activities.Clear();
                    GroupedActivities.Clear();

                    await LoadFeed();

                    if (Activities.Any())
                    {
                        var activityRepo = new DataAccess.ActivityRepository();
                        activityRepo.DeleteActivitiesExcept(Activities);
                    }
                    else
                    {
                        await LoadFeedFromRepository();
                    }

                    EmptyState.Visibility = Activities.Any() ? Visibility.Collapsed : Visibility.Visible;
                }

                var errorQueryIndex = e.Uri.Query.IndexOf("error=");
                if (errorQueryIndex != -1)
                {
                    BeginStravaAuthorization();
                }
            }
            else
            {
                BrowserProgress.IsRunning = true;
            }
        }

        private void Browser_Navigated(object sender, NavigatedEventHandler e)
        {
            BrowserProgress.IsRunning = false;
        }

        private void UpdateLastActivity()
        {
            var repo = new DataAccess.ActivityRepository();

            var updatedActivity = repo.GetActivity(LastActivityId);
            for (var i = 0; i < Activities.Count; i++)
            {
                var oldActivity = Activities[i];
                if (oldActivity.Id == LastActivityId)
                {
                    if (oldActivity.CommentCount != updatedActivity.CommentCount ||
                        oldActivity.KudosCount != updatedActivity.KudosCount ||
                        oldActivity.HasKudoed != updatedActivity.HasKudoed ||
                        oldActivity.Name != updatedActivity.Name)
                    {
                        oldActivity.Name = updatedActivity.Name;
                        oldActivity.CommentCount = updatedActivity.CommentCount;
                        oldActivity.KudosCount = updatedActivity.KudosCount;
                        oldActivity.HasKudoed = updatedActivity.HasKudoed;
                        oldActivity.NotifyPropertyChanged("Name");
                        oldActivity.NotifyPropertyChanged("CommentCount");
                        oldActivity.NotifyPropertyChanged("KudosCount");
                        oldActivity.NotifyPropertyChanged("HasKudoed");
                    }
                    break;
                }
            }
        }

        private async Task LoadFeedFromRepository()
        {
            try
            {
                Feed.Opacity = .25;
                Feed.IsEnabled = false;
                Progress.IsRunning = true;
                IsCurrentlyLoading = true;

                var cachedActivities = await Task.Run<List<Strava.Activities.Activity>>(() =>
                {
                    var repo = new DataAccess.ActivityRepository();
                    var cachedActivites = repo.GetActivities();
                    if (cachedActivites.Length != 0)
                    {
                        if (DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends == false)
                            cachedActivites = cachedActivites.Where(w => w.Athlete.Id == DataAccess.StreedApplicationSettings.AuthenticatedAthleteId).ToArray();

                        var sorted = cachedActivites.OrderByDescending(o => o.StartDateTime).ToArray();
                        return new List<Strava.Activities.Activity>(sorted);
                    }
                    return null;
                });

                if (cachedActivities == null)
                    return;

                //only load activities that were originally loaded by the Feed (main) page
                //activities may be cached as a result of looking at the activities of athletes
                //you don't follow via that Related athletes and Segment pages
                cachedActivities = cachedActivities.Where(w => w.WasLoadedByFeedPage == true).ToList();

                UpdateGroupedActivities(cachedActivities);

                cachedActivities.ForEach(a =>
                {
                    Activities.Add(a);
                    a.Athlete.LoadProfileImages();
                    a.Map.LoadSummaryMaps();
                });
            }
            finally
            {
                Feed.Opacity = 1;
                Feed.IsEnabled = true;
                Progress.IsRunning = false;
                IsCurrentlyLoading = false;
                LastUpdated.Text = string.Format("LAST UPDATED {0}", DataAccess.StreedApplicationSettings.RefreshedFeedAgo.ToUpper());
            }
        }

        private async Task LoadFeed()
        {
            try
            {
                Feed.Opacity = .25;
                Feed.IsEnabled = false;
                Progress.IsRunning = true;
                IsCurrentlyLoading = true;

                Strava.Activities.Activity[] freshActivities = null;

                try
                {
                    if (DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends)
                        freshActivities = await Strava.Api.Activities.GetFriendActivities(DataAccess.StreedApplicationSettings.AccessToken.Token, CurrentPage, ActivitiesPerPage);
                    else
                        freshActivities = await Strava.Api.Activities.GetAthleteActivities(DataAccess.StreedApplicationSettings.AccessToken.Token, CurrentPage, ActivitiesPerPage);
                }
                catch (Strava.Api.AuthorizationFailedException)
                {
                    DataAccess.StreedApplicationSettings.AccessToken = null;
                    BeginStravaAuthorization();
                    return;
                }
                catch (Strava.Api.RateLimitExceededException)
                {
                    DisplayError("Strava Api Limit", "Please try again in 15 minutes");
                    return;
                }

                if (freshActivities == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                DataAccess.StreedApplicationSettings.LastTimeRefreshedFeed = DateTime.UtcNow;
                CurrentPage++;
                IsMoreToLoad = !(freshActivities.Length < ActivitiesPerPage);

                var sorted = freshActivities.OrderByDescending(o => o.StartDateTime).ToList();
                sorted.ForEach(a => Activities.Add(a));

                UpdateGroupedActivities(sorted);

                var activityRepository = new DataAccess.ActivityRepository();
                var athleteRepository = new DataAccess.AthleteRepository();

                sorted.ToList().ForEach(a =>
                {
                    a.WasLoadedByFeedPage = true;
                    var existingActivity = activityRepository.GetActivity(a.Id);
                    if (existingActivity == null)
                    {
                        activityRepository.InsertActivity(a, athleteRepository);
                    }
                    else
                    {
                        activityRepository.UpdateActivity(a, athleteRepository);
                    }

                    if (a.Athlete.Id == DataAccess.StreedApplicationSettings.AuthenticatedAthleteId)
                        a.Athlete = DataAccess.StreedApplicationSettings.AccessToken.Athlete;
                    a.Athlete.LoadProfileImages();
                    a.Map.LoadSummaryMaps();
                });
            }
            finally
            {
                Feed.Opacity = 1;
                Feed.IsEnabled = true;
                Progress.IsRunning = false;
                IsCurrentlyLoading = false;
            }
        }

        private void UpdateGroupedActivities(List<Strava.Activities.Activity> activities)
        {
            if (activities == null || activities.Count == 0)
                return;

            var today = DateTime.Today.ToUniversalTime().Date;

            var maxActivityStartDateLocal = activities.Max(m => m.StartDateTimeLocal.Date);
            if (today < maxActivityStartDateLocal)
            { 
                //newest activity start date is in the future when compared to the device date
                //device date is possibly not set correctly
                //check if we have a server datetime from the last Api request and use that instead
                if (Strava.Api.RestRequest.ServerDateTime != DateTime.MinValue)
                {
                    today = Strava.Api.RestRequest.ServerDateTime.Date;
                }
                else
                { 
                    //as a backup, use the newest activty
                    today = maxActivityStartDateLocal;
                }
            }

            var groups = activities.GroupBy(g => g.StartDateTimeLocal.Date).ToList();
            foreach (var g in groups)
            {
                var diff = today - g.Key.Date;
                if (diff.Days == 0)
                {
                    if (GroupedActivities.Any(a => a.Title == "TODAY"))
                    {
                        GroupedActivities.Where(w => w.Title == "TODAY").Single().AddRange(g.OrderByDescending(o => o.StartDateTime).ToArray());
                    }
                    else
                    {
                        GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection("TODAY", g.Key.Date, g.OrderByDescending(o => o.StartDateTime).ToArray()));
                    }
                }
                else if (diff.Days == 1)
                {
                    if (GroupedActivities.Any(a => a.Title == "YESTERDAY"))
                    {
                        GroupedActivities.Where(w => w.Title == "YESTERDAY").Single().AddRange(g.OrderByDescending(o => o.StartDateTime).ToArray());
                    }
                    else
                    {
                        GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection("YESTERDAY", g.Key.Date, g.OrderByDescending(o => o.StartDateTime).ToArray()));
                    }
                }
                else if (diff.Days > 1)
                {
                    if (GroupedActivities.Any(a => a.Title == g.Key.Date.ToShortDateString()))
                    {
                        GroupedActivities.Where(w => w.Title == g.Key.Date.ToShortDateString()).Single().AddRange(g.OrderByDescending(o => o.StartDateTime).ToArray());
                    }
                    else
                    {
                        if (GroupedActivities.Any(w => w.TitleDateTime.Date == g.Key.AddDays(1).Date))
                        {
                            var previousGroup = GroupedActivities.Where(w => w.TitleDateTime.Date == g.Key.AddDays(1).Date).Single();
                            var index = GroupedActivities.IndexOf(previousGroup);
                            if (index == (GroupedActivities.Count - 1))
                                GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection(g.Key.Date.ToShortDateString(), g.Key.Date, g.OrderByDescending(o => o.StartDateTime).ToArray()));
                            else
                                GroupedActivities.Insert(index + 1, new Strava.Activities.GroupedActivityObservableCollection(g.Key.Date.ToShortDateString(), g.Key.Date, g.OrderByDescending(o => o.StartDateTime).ToArray()));
                        }
                        else
                        {
                            GroupedActivities.Add(new Strava.Activities.GroupedActivityObservableCollection(g.Key.Date.ToShortDateString(), g.Key.Date, g.OrderByDescending(o => o.StartDateTime).ToArray()));
                        }
                            
                    }
                }
            }
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            LastActivityId = (long)((Button)sender).Tag;
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/CommentsPage.xaml?id={0}", LastActivityId), UriKind.Relative));
        }

        private async void KudosButton_Click(object sender, RoutedEventArgs e)
        {
            KudosProgress.IsRunning = true;

            try
            {
                IsCurrentlyHandlingKudosOrComment = true;

                var kudosActivityId = (long)((Button)sender).Tag;

                var id = (long)((Button)sender).Tag;
                await Strava.Api.Activities.GiveKudos(id, DataAccess.StreedApplicationSettings.AccessToken.Token);

                var activity = Activities.Where(w => w.Id == kudosActivityId).Single();
                activity.KudosCount++;
                activity.HasKudoed = true;
                activity.NotifyPropertyChanged("KudosCount");
                activity.NotifyPropertyChanged("HasKudoed");
                activity.NotifyPropertyChanged("CanKudos");

                new DataAccess.ActivityRepository().UpdateActivityKudosCount(id, activity.KudosCount);
            }
            catch (Strava.Api.AuthorizationFailedException)
            {
                DataAccess.StreedApplicationSettings.AccessToken = null;
                BeginStravaAuthorization();
            }
            catch (Strava.Api.RateLimitExceededException)
            {
                DisplayError("Strava Api Limit", "Please try again in 15 minutes");
            }
            finally 
            {
                IsCurrentlyHandlingKudosOrComment = false;
                KudosProgress.IsRunning = false;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LastActivityId = 0;
            CurrentPage = 1;
            LastUpdated.Text = string.Empty;

            var oldActivities = new List<Strava.Activities.Activity>(Activities);
            Activities.Clear();
            var oldGroupedActivites = new List<Strava.Activities.GroupedActivityObservableCollection>(GroupedActivities);
            GroupedActivities.Clear();

            Progress.Content = "Loading...";
            await LoadFeed();

            if (Activities.Any())
            {
                var activityRepo = new DataAccess.ActivityRepository();
                activityRepo.DeleteActivitiesExcept(Activities);
            }
            else
            {
                Activities.AddRange(oldActivities);
                oldGroupedActivites.ForEach(g => GroupedActivities.Add(g));
            }

            EmptyState.Visibility = Activities.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void Feed_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            var activity = e.Container.Content as Strava.Activities.Activity;
            if (activity != null)
            {
                int offset = 1;
                if (IsMoreToLoad && IsCurrentlyLoading == false && Activities.Count - Activities.IndexOf(activity) <= offset)
                {
                    Progress.Content = "Loading more...";
                    await LoadFeed();
                    LastUpdated.Text = string.Format("LAST UPDATED {0}", DataAccess.StreedApplicationSettings.RefreshedFeedAgo.ToUpper());
                }
            }
        }

        private void CaptureKudosButton_Click(object sender, RoutedEventArgs e)
        {
            //eat this so the Feed doesn't get Click when
            //the Kudos button is disabled
        }

        private void Activity_Click(object sender, RoutedEventArgs e)
        {
            LastActivityId = (long)((FrameworkElement)sender).Tag;
            var athleteIdOfActivity = Activities.Where(w => w.Id == LastActivityId).Select(s => s.Athlete.Id).Single();
            if (athleteIdOfActivity == DataAccess.StreedApplicationSettings.AuthenticatedAthleteId)
                this.NavigationService.Navigate(new Uri(string.Format("/Pages/ActivityPage.xaml?id={0}", LastActivityId), UriKind.Relative));
            else
                this.NavigationService.Navigate(new Uri(string.Format("/Pages/BasicActivityPage.xaml?id={0}", LastActivityId), UriKind.Relative));
        }

        private void ApplicationBarSettings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
        }

        private async void FilterListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FilterPopup.Visibility = System.Windows.Visibility.Collapsed;
            FilterPopup.IsOpen = false;

            FilterListBox.Tap -= FilterListBox_Tap;

            var filterFollowing = FilterListBox.SelectedIndex == 0;
            if (filterFollowing != DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends)
            {
                DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends = filterFollowing;
                FilterText.Text = DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends ? "Following" : "Me";
                LastActivityId = 0;
                CurrentPage = 1;
                LastUpdated.Text = string.Empty;

                var oldActivities = new List<Strava.Activities.Activity>(Activities);
                Activities.Clear();
                var oldGroupedActivites = new List<Strava.Activities.GroupedActivityObservableCollection>(GroupedActivities);
                GroupedActivities.Clear();
                
                await LoadFeed();

                if (Activities.Any())
                {
                    var activityRepo = new DataAccess.ActivityRepository();
                    activityRepo.DeleteActivitiesExcept(Activities);
                }
                else
                {
                    Activities.AddRange(oldActivities);
                    oldGroupedActivites.ForEach(g => GroupedActivities.Add(g));
                }

                EmptyState.Visibility = Activities.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void AthleteButton_Click(object sender, RoutedEventArgs e)
        {
            var athleteId = (long)((FrameworkElement)sender).Tag;
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/AthletePage.xaml?id={0}", athleteId), UriKind.Relative));
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            if (FilterListBox.Items.Count == 0)
            {
                FilterListBox.Items.Add("Following");
                FilterListBox.Items.Add("Me");
                FilterListBox.SelectedIndex = DataAccess.StreedApplicationSettings.IsFeedFilteringByFriends ? 0 : 1;
            }

            FilterListBox.Tap += FilterListBox_Tap;

            FilterPopup.Visibility = System.Windows.Visibility.Visible;
            FilterPopup.IsOpen = true;
        }
    }
}