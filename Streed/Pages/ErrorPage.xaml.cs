using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Text;
using System.Globalization;

namespace Streed.Pages
{
    public partial class ErrorPage : PhoneApplicationPage
    {
        private Models.SerializableException UnhandledException { get; set; }

        public ErrorPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //clear the back stack
            //if the user hits the back key, the app will exit
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            if (UnhandledException == null)
            {
                UnhandledException = DataAccess.StreedApplicationSettings.UnhandledException;
                if (UnhandledException != null)
                {
                    Message.Text = UnhandledException.Message;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //email support
            var emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Streed Support";
            emailComposeTask.To = "ontheshelfsoftware@gmail.com";

            if (UnhandledException != null)
            {
                var error = UnhandledException;
                var sb = new StringBuilder();

                sb.Append("[tell us what happened?]\n\n\n\n");
                sb.AppendFormat(CultureInfo.CurrentCulture, "message: {0}\n\n", error.Message);
                sb.AppendFormat(CultureInfo.CurrentCulture, "stackTrace: {0}\n\n", error.StackTrace);

                if (error.Uri != null)
                    sb.AppendFormat(CultureInfo.CurrentCulture, "uri: {0}\n\n", error.Uri);

                var innerException = error.InnerException;

                var tabcount = 1;
                var tb = new StringBuilder();
                while (innerException != null)
                {
                    for (var t = 0; t < tabcount; t++)
                        tb.Append("\t");

                    var tabs = tb.ToString();

                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0}innerException message: {1}\n", tabs, innerException.Message);
                    sb.AppendFormat(CultureInfo.CurrentCulture, "{0}innerException stackTrace: {1}\n\n", tabs, innerException.StackTrace);

                    if (innerException.Uri != null)
                        sb.AppendFormat(CultureInfo.CurrentCulture, "{0}innerException uri: {1}\n\n", tabs, innerException.Uri);

                    innerException = innerException.InnerException;
                    tabcount++;
                }

                emailComposeTask.Body = sb.ToString();
            }

            emailComposeTask.Show();
        }
    }
}