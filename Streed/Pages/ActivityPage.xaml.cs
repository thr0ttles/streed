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
using Telerik.Windows.Controls;
using Telerik.Charting;

namespace Streed.Pages
{
    public partial class ActivityPage : PhoneApplicationPage
    {
        private ViewModels.ActivityViewModel ViewModel { get; set; }

        public ActivityPage()
        {
            InitializeComponent();

            ViewModel = new ViewModels.ActivityViewModel();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                ViewModel.Refresh();
                ViewModel.Activity.Athlete.LoadProfileImages();
                ViewModel.Activity.Map.LoadSummaryMaps();

                return;
            }

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                try
                {
                    Progress.IsRunning = true;

                    var activityId = long.Parse(id);

                    try
                    {
                        var loaded = await ViewModel.Load(activityId);

                        if (loaded == false)
                        {
                            DisplayError("Network Unavailable", "Please check your data connection");
                            return;
                        }
                    }
                    catch (Strava.Api.AuthorizationFailedException)
                    {
                        DataAccess.StreedApplicationSettings.AccessToken = null;
                        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                        return;
                    }
                    catch (Strava.Api.RateLimitExceededException)
                    {
                        DisplayError("Strava Api Limit", "Please try again in 15 minutes");
                        return;
                    }

                    ViewModel.Activity.Athlete.LoadProfileImages();
                    ViewModel.Activity.Map.LoadSummaryMaps();

                    if (ViewModel.SegmentEfforts == null)
                        EmptyState.Visibility = System.Windows.Visibility.Visible;
                    if (ViewModel.SegmentEfforts != null && ViewModel.SegmentEfforts.Count() == 0)
                        EmptyState.Visibility = System.Windows.Visibility.Visible;

                    //set HeartRateData and PowerData to empty collections so the UI
                    //can handle the empty state when there isn't data to show
                    if (ViewModel.HeartRateData == null)
                        ViewModel.HeartRateData = new System.Collections.ObjectModel.ObservableCollection<Models.HeartRateZoneData>();
                    if (ViewModel.PowerData == null)
                        ViewModel.PowerData = new System.Collections.ObjectModel.ObservableCollection<Models.PowerDistributionData>();

                    //HACK:this tries to ensure the labels on the power bar chart are not clipped
                    if (ViewModel.PowerData.Any())
                    {
                        var max = ViewModel.PowerData.Max(m => m.Time);
                        horizontalAxis.Maximum = max * 1.25;
                    }
                }
                finally
                {
                    Progress.IsRunning = false;
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/Pages/EditActivityPage.xaml?id={0}", ViewModel.Activity.Id), UriKind.Relative));
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/Pages/CommentsPage.xaml?id={0}", ViewModel.Activity.Id), UriKind.Relative));
        }

        private async void KudosButton_Click(object sender, RoutedEventArgs e)
        {
            KudosProgress.IsRunning = true;

            try
            {
                var id = (long)((Button)sender).Tag;
                await Strava.Api.Activities.GiveKudos(id, DataAccess.StreedApplicationSettings.AccessToken.Token);

                ViewModel.Activity.KudosCount++;
                ViewModel.Activity.HasKudoed = true;
                new DataAccess.ActivityRepository().UpdateActivityKudosCount(id, ViewModel.Activity.KudosCount);

                ViewModel.Activity.NotifyPropertyChanged("KudosCount");
                ViewModel.Activity.NotifyPropertyChanged("HasKudoed");
                ViewModel.Activity.NotifyPropertyChanged("CanKudos");
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

        private void Segment_Click(object sender, RoutedEventArgs e)
        {
            var segmentEffortId = ((FrameworkElement)sender).Tag;
            NavigationService.Navigate(new Uri(string.Format("/Pages/SegmentEffortPage.xaml?id={0}", segmentEffortId), UriKind.Relative));
        }

        private void RelatedAthletesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(string.Format("/Pages/RelatedAthletesPage.xaml?id={0}", ViewModel.Activity.Id), UriKind.Relative));
        }

        private void AthleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/AthletePage.xaml?id={0}", ViewModel.Activity.Athlete.Id), UriKind.Relative));
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }

        private object _selected = null;
        private void ListBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ViewModel.Activity.Laps != null && ViewModel.Activity.Laps.Count() == 1)
            {
                e.Handled = true;
                return;
            }

            var listbox = (ListBox)sender;
            if (listbox.SelectedItem == _selected)
            {
                listbox.SelectedItem = null;
                _selected = null;
            }
            else
            {
                _selected = listbox.SelectedItem;
            }
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Activity != null && ViewModel.Activity.Map != null)
            {
                string polyline = null;
                if (string.IsNullOrWhiteSpace(ViewModel.Activity.Map.Polyline) == false)
                    polyline = ViewModel.Activity.Map.Polyline;
                else if (string.IsNullOrWhiteSpace(ViewModel.Activity.Map.SummaryPolyline) == false)
                    polyline = ViewModel.Activity.Map.SummaryPolyline;

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