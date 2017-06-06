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
    public partial class EditActivityPage : PhoneApplicationPage
    {
        private long ActivityId { get; set; }
        private Strava.Activities.Activity Activity { get; set; }

        public EditActivityPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ActivityType.ItemsSource = Enum.GetValues(typeof(Strava.Activities.ActivityType));

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                ActivityId = long.Parse(id);

                Progress.IsRunning = true;
                ContentPanel.Opacity = .25;
                ContentPanel.IsHitTestVisible = false;

                LoadActivity();

                Progress.IsRunning = false;
                ContentPanel.Opacity = 1;
                ContentPanel.IsHitTestVisible = true;
            }
        }

        private void LoadActivity()
        {
            var repo = new DataAccess.ActivityRepository();
            Activity = repo.GetActivity(ActivityId);

            if (Activity == null)
                throw new KeyNotFoundException(string.Format("Unable to load activity, does not exist. {0}", ActivityId));

            if (string.IsNullOrWhiteSpace(Activity.Description)) Activity.Description = "Add a description...";
            this.DataContext = Activity;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Progress.IsRunning = true;
                ContentPanel.Opacity = .25;
                ContentPanel.IsHitTestVisible = false;

                if (Activity.Description == "Add a description...") Activity.Description = string.Empty;

                var activity = await Strava.Api.Activities.UpdateActivity(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token, Activity.Name, Activity.Description, Activity.Private);

                if (activity == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                Activity = activity;
                new DataAccess.ActivityRepository().UpdateNameDescriptionIsPrivate(activity.Id, activity.Name, activity.Description, activity.Private);

                if (NavigationService.CanGoBack) NavigationService.GoBack();
                else NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
                ContentPanel.Opacity = 1;
                ContentPanel.IsHitTestVisible = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack) NavigationService.GoBack();
            else NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void DescriptionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Description.Text == "Add a description...") Description.Text = string.Empty;
        }

        private void DescriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Description.Text)) Description.Text = "Add a description...";
            Description.Text = Description.Text.Trim();
        }

        private void ActivityName_LostFocus(object sender, RoutedEventArgs e)
        {
            ActivityName.Text = ActivityName.Text.Trim();
        }

        private void DisplayError(string title, string message)
        {
            ErrorTitle.Text = title;
            ErrorMessage.Text = message;
            ErrorBanner.Opacity = 1;
            ErrorBanner.Visibility = System.Windows.Visibility.Visible;
            ErrorBannerFadeAnimation.Begin();
        }
    }
}