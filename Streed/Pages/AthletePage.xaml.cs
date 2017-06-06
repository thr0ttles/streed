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
using System.Collections.ObjectModel;

namespace Streed.Pages
{
    public partial class AthletePage : PhoneApplicationPage
    {
        private long AthleteId { get; set; }
        private Strava.Athletes.Athlete Athlete { get; set; }
        private int CurrentPage { get; set; }
        private bool IsMoreToLoad { get; set; }
        private readonly int PerPage = 20;
        private ObservableCollection<Strava.Segments.SegmentEffort> Koms { get; set; }
        private bool IsCurrentlyLoading { get; set; }

        public AthletePage()
        {
            InitializeComponent();

            CurrentPage = 1;
            IsMoreToLoad = false;
            IsCurrentlyLoading = false;
            Koms = new ObservableCollection<Strava.Segments.SegmentEffort>();
            KomList.ItemsSource = Koms;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                AthleteId = long.Parse(id);
            }

            if (Athlete == null)
            {
                ContentPanel.Opacity = .25;
                ContentPanel.IsHitTestVisible = false;
                Progress.IsRunning = true;

                await LoadAthlete();
                await LoadKoms();

                ContentPanel.Opacity = 1;
                ContentPanel.IsHitTestVisible = true;
                Progress.IsRunning = false;
            }
        }

        private async Task LoadAthlete()
        {
            var athleteRepository = new DataAccess.AthleteRepository();
            Athlete = athleteRepository.GetAthlete(AthleteId);

            if (Athlete != null && Athlete.ResourceState > 1)
            {
                Athlete.LoadProfileImages();
                DataContext = Athlete;
                var repo = new DataAccess.AthleteRepository();
                repo.InsertAthlete(Athlete);

                return;
            }

            try
            {
                Athlete = await Strava.Api.Athletes.GetAthlete(AthleteId, DataAccess.StreedApplicationSettings.AccessToken.Token);
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

            if (Athlete == null)
            {
                DisplayError("Network Unavailable", "Please check your data connection");
                return;
            }

            Athlete.LoadProfileImages();
            DataContext = Athlete;
            new DataAccess.AthleteRepository().InsertAthlete(Athlete);
        }

        private async Task LoadKoms()
        {
            try
            {
                IsCurrentlyLoading = true;

                try
                {
                    var segmentEfforts = await Strava.Api.Athletes.GetKoms(AthleteId, DataAccess.StreedApplicationSettings.AccessToken.Token, CurrentPage, PerPage);
                    if (segmentEfforts == null)
                    {
                        DisplayError("Network Unavailable", "Please check your data connection");
                        return;
                    }

                    CurrentPage++;
                    IsMoreToLoad = (segmentEfforts.Count() == PerPage);
                    segmentEfforts.ToList().ForEach(se => Koms.Add(se));
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
            finally
            {
                IsCurrentlyLoading = false;
            }
        }

        private void Segment_Click(object sender, RoutedEventArgs e)
        {
            var segmentEffortId = ((FrameworkElement)sender).Tag;
            NavigationService.Navigate(new Uri(string.Format("/Pages/SegmentEffortPage.xaml?id={0}", segmentEffortId), UriKind.Relative));
        }

        private async void KomList_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            var segmentEffort = e.Container.Content as Strava.Segments.SegmentEffort;
            if (segmentEffort != null)
            {
                int offset = 1;
                if (IsMoreToLoad && IsCurrentlyLoading == false && Koms.Count - Koms.IndexOf(segmentEffort) <= offset)
                {
                    ContentPanel.Opacity = .25;
                    ContentPanel.IsHitTestVisible = false;
                    Progress.IsRunning = true;

                    await LoadKoms();
                    
                    ContentPanel.Opacity = 1;
                    ContentPanel.IsHitTestVisible = true;
                    Progress.IsRunning = false;
                }
            }
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}