using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Streed.Pages
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        private int ActivityId { get; set; }
        private int CurrentPage { get; set; }
        private bool IsCurrentlyLoading { get; set; }
        private bool IsMoreToLoad { get; set; }
        private readonly int CommentsPerPage = 30;
        private ObservableCollection<Strava.Comments.Comment> Comments { get; set; }
        private bool HasLoaded { get; set; }

        public CommentsPage()
        {
            InitializeComponent();

            HasLoaded = false;
            Comments = new ObservableCollection<Strava.Comments.Comment>();
            CommentsFeed.ItemsSource = Comments;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string id;
            if (this.NavigationContext.QueryString.TryGetValue("id", out id))
            {
                ActivityId = int.Parse(id);

                if (HasLoaded == false)
                {
                    SetActivityTitleOnPage(ActivityId);

                    CurrentPage = 1;
                    Comments.Clear();
                    IsMoreToLoad = true;

                    await LoadComments();
                }
            }
        }

        private void SetActivityTitleOnPage(long id)
        {
            var repo = new DataAccess.ActivityRepository();
            var activity = repo.GetActivity(id);
            if (activity != null)
            {
                ActivityName.Text = activity.Name;
            }
        }

        private async Task LoadComments()
        {
            try
            {
                IsCurrentlyLoading = true;
                CommentsFeed.IsEnabled = false;
                AddComment.IsEnabled = false;
                Progress.IsRunning = true;

                var comments = await Strava.Api.Activities.GetComments(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token, CurrentPage, CommentsPerPage);

                if (comments == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                HasLoaded = true;
                CurrentPage++;
                IsMoreToLoad = !(comments.Length < CommentsPerPage);

                comments.ToList().ForEach(c =>
                    {
                        Comments.Add(c);
                        c.Athlete.LoadProfileImages();
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
            finally
            {
                IsCurrentlyLoading = false;
                CommentsFeed.IsEnabled = true;
                AddComment.IsEnabled = true;
                Progress.IsRunning = false;
            }
        }

        private async Task PostComment(string comment)
        {
            CommentProgress.IsRunning = true;

            try
            {
                CommentsFeed.IsEnabled = false;
                AddComment.IsEnabled = false;

                var addedComment = await Strava.Api.Activities.InsertComment(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token, comment);

                if (addedComment == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                Comments.Add(addedComment);
                addedComment.Athlete.LoadProfileImages();

                await UpdateActivityCommentCount();

                this.CommentsFeed.UpdateLayout();
                this.CommentsFeed.ScrollTo(addedComment);
                this.AddComment.Text = "";
                this.Focus();
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
                CommentProgress.IsRunning = false;
                CommentsFeed.IsEnabled = true;
                AddComment.IsEnabled = true;
            }
        }

        private async Task UpdateActivityCommentCount()
        {
            try
            {
                var activity = await Strava.Api.Activities.GetActivity(ActivityId, DataAccess.StreedApplicationSettings.AccessToken.Token);

                if (activity == null)
                {
                    DisplayError("Network Unavailable", "Please check your data connection");
                    return;
                }

                var repo = new DataAccess.ActivityRepository();
                repo.UpdateActivityCommentCount(activity.Id, Comments.Count);
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

        private void AddComment_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.AddComment.Text == "Post a comment...")
                this.AddComment.Text = "";
        }

        private void AddComment_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.AddComment.Text))
                this.AddComment.Text = "Post a comment...";
        }

        private async void AddComment_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                await PostComment(this.AddComment.Text);
            }
        }

        private async void CommentsFeed_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            var comment = e.Container.Content as Strava.Comments.Comment;
            if (comment != null)
            { 
                int offset = 2;
                if (IsMoreToLoad && IsCurrentlyLoading == false && Comments.Count - Comments.IndexOf(comment) <= offset)
                    await LoadComments();
            }
        }
        
        private void AthleteButton_Click(object sender, RoutedEventArgs e)
        {
            var id = ((FrameworkElement)sender).Tag;
            this.NavigationService.Navigate(new Uri(string.Format("/Pages/AthletePage.xaml?id={0}", id), UriKind.Relative));
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