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
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataAccess.StreedApplicationSettings.HasAccessToken)
            {
                var authenticatedAthlete = DataAccess.StreedApplicationSettings.AccessToken.Athlete;
                authenticatedAthlete.LoadProfileImages();
                this.DataContext = authenticatedAthlete;
            }
        }

        private async void Deauthorize_Click(object sender, RoutedEventArgs e)
        {
            await Strava.Api.Authentication.Deauthorize(DataAccess.StreedApplicationSettings.AccessToken.Token);

            DataAccess.StreedApplicationSettings.AccessToken = null;

            var repo = new DataAccess.ActivityRepository();
            repo.DeleteAllActivities(new DataAccess.AthleteRepository());

            if (NavigationService.CanGoBack) NavigationService.GoBack();
            else NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}
