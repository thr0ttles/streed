using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Streed.Pages
{
    public partial class RelatedAthletesPage : PhoneApplicationPage
    {
        private bool IsCurrentlyHandlingKudosOrComment { get; set; }
        private long LastActivityId { get; set; }
        private long ParentActivityId { get; set; }
        private ViewModels.RelatedAthletesViewModel ViewModel { get; set; }
        private bool HasAttemptedToLoad { get; set; }

        public RelatedAthletesPage()
        {
            InitializeComponent();

            ViewModel = new ViewModels.RelatedAthletesViewModel();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                ParentActivityId = long.Parse(id);

                if (HasAttemptedToLoad)
                    return;

                try
                {
                    Progress.IsRunning = true;
                    var relatedActivities = await Strava.Api.Activities.GetRelatedActivities(ParentActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token);
                    if (relatedActivities != null)
                    {
                        relatedActivities.ToList().ForEach(a =>
                            {
                                a.Athlete.LoadProfileImages();
                                ViewModel.RelatedActivities.Add(a);
                            });
                    }
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
                    Progress.IsRunning = false;
                    HasAttemptedToLoad = true;
                }
            }

        }

        private async void KudosButton_Click(object sender, RoutedEventArgs e)
        {
            KudosProgress.IsRunning = true;

            try
            {
                IsCurrentlyHandlingKudosOrComment = true;

                var kudosActivityId = (long)((Button)sender).Tag;

                await Strava.Api.Activities.GiveKudos(kudosActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                var activity = ViewModel.RelatedActivities.Where(w => w.Id == kudosActivityId).Single();
                activity.KudosCount++;
                activity.HasKudoed = true;
                activity.NotifyPropertyChanged("KudosCount");
                activity.NotifyPropertyChanged("HasKudoed");
                activity.NotifyPropertyChanged("CanKudos");

                new DataAccess.ActivityRepository().UpdateActivityKudosCount(activity.Id, activity.KudosCount);
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
                IsCurrentlyHandlingKudosOrComment = false;
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
            var athleteIdOfActivity = ViewModel.RelatedActivities.Where(w => w.Id == LastActivityId).Select(s => s.Athlete.Id).Single();
            if (athleteIdOfActivity == DataAccess.StreedApplicationSettings.AuthenticatedAthleteId)
                this.NavigationService.Navigate(new Uri(string.Format("/Pages/ActivityPage.xaml?id={0}", LastActivityId), UriKind.Relative));
            else
                this.NavigationService.Navigate(new Uri(string.Format("/Pages/BasicActivityPage.xaml?id={0}", LastActivityId), UriKind.Relative));
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            LastActivityId = (long)((FrameworkElement)sender).Tag;
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/CommentsPage.xaml?id={0}", LastActivityId), UriKind.Relative));
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

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}