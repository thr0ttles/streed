using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Pages
{
    public partial class BasicActivityPage : PhoneApplicationPage
    {
        private long ActivityId { get; set; }
        private Strava.Activities.Activity Activity { get; set; }

        public BasicActivityPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                ActivityId = long.Parse(id);
                LoadActivity();
            }
        }

        private async void LoadActivity()
        {
            try
            {
                ContentPanel.Opacity = 0;
                ContentPanel.IsHitTestVisible = false;
                Progress.IsRunning = true;

                var repo = new DataAccess.ActivityRepository();
                var activity = repo.GetActivity(ActivityId);

                var loadFromApi = false;
                if (activity == null) loadFromApi = true;
                else loadFromApi = (activity.ResourceState < 2);

                if (loadFromApi == false)
                {
                    activity.Athlete.LoadProfileImages();
                    activity.Map.LoadSummaryMaps();
                    Activity = activity;
                    this.DataContext = Activity;
                }

                if (loadFromApi)
                {
                    try
                    {
                        Activity = await Strava.Api.Activities.GetActivity(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                        if (Activity == null)
                        {
                            DisplayError("Network Unavailable", "Please check your data connection");
                            return;
                        }

                        if (activity != null)
                        {
                            repo.UpdateActivity(Activity);
                        }
                        else
                        {
                            repo.InsertActivity(Activity);
                        }

                        this.DataContext = Activity;
                        Activity.Athlete.LoadProfileImages();
                        Activity.Map.LoadSummaryMaps();
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
            }
            finally
            {
                ContentPanel.Opacity = 1;
                ContentPanel.IsHitTestVisible = true;
                Progress.IsRunning = false;
            }
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/CommentsPage.xaml?id={0}", ActivityId), UriKind.Relative));
        }

        private async void KudosButton_Click(object sender, RoutedEventArgs e)
        {
            KudosProgress.IsRunning = true;

            try
            {
                await Strava.Api.Activities.GiveKudos(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                Activity.KudosCount++;
                Activity.HasKudoed = true;
                var repo = new DataAccess.ActivityRepository();
                new DataAccess.ActivityRepository().UpdateActivityKudosCount(Activity.Id, Activity.KudosCount);

                Activity.NotifyPropertyChanged("KudosCount");
                Activity.NotifyPropertyChanged("HasKudoed");
                Activity.NotifyPropertyChanged("CanKudos");
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
            finally 
            {
                KudosProgress.IsRunning = false;
            }
        }

        private void RelatedAthletesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/Pages/RelatedAthletesPage.xaml?id={0}", ActivityId), UriKind.Relative));
        }

        private void AthleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/AthletePage.xaml?id={0}", Activity.Athlete.Id), UriKind.Relative));
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (Activity != null && Activity.Map != null)
            {
                string polyline = null;
                if (string.IsNullOrWhiteSpace(Activity.Map.Polyline) == false)
                    polyline = Activity.Map.Polyline;
                else if (string.IsNullOrWhiteSpace(Activity.Map.SummaryPolyline) == false)
                    polyline = Activity.Map.SummaryPolyline;

                if (string.IsNullOrWhiteSpace(polyline) == false)
                {
                    polyline = polyline.Replace(@"\", "%5c");
                    NavigationService.Navigate(new Uri(string.Format("/Pages/MapPage.xaml?polyline={0}", polyline), UriKind.Relative));
                }
            }
        }

        private void TimeGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (MovingTime.Visibility == System.Windows.Visibility.Visible) TimeGridFlipAnimationToBack.Begin();
            else TimeGridFlipAnimationToFront.Begin();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}