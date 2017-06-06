using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Streed.Strava.Comments
{
    [DataContract]
    public class Comment : INotifyPropertyChanged
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "activity_id")]
        public long ActivityId { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "athlete")]
        public Strava.Athletes.Athlete Athlete { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedDateTime { get; set; }

        public string CreatedAgo
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParse(CreatedDateTime, out dt))
                {
                    dt = dt.ToUniversalTime();
                    var diff = (DateTime.UtcNow - dt);

                    if ((int)diff.TotalDays > 730)
                    {
                        return string.Format("{0} years ago", (int)(diff.TotalDays / 365));
                    }
                    else if ((int)diff.TotalDays > 365)
                    {
                        return "over a year ago";
                    }
                    else if ((int)diff.TotalDays > 30)
                    {
                        return string.Format("{0} months ago", (int)(diff.TotalDays / 30));
                    }
                    else if ((int)diff.TotalDays > 1)
                    {
                        return string.Format("{0} days ago", (int)diff.TotalDays);
                    }
                    else if ((int)diff.TotalDays == 1)
                    {
                        return "1 day ago";
                    }
                    else if ((int)diff.TotalHours > 1)
                    {
                        return string.Format("{0} hours ago", (int)diff.TotalHours);
                    }
                    else if ((int)diff.TotalHours == 1)
                    {
                        return "1 hour ago";
                    }
                    else if ((int)diff.TotalMinutes > 1)
                    {
                        return string.Format("{0} minutes ago", (int)diff.TotalMinutes);
                    }
                    else if ((int)diff.TotalMinutes == 1)
                    {
                        return "over a minute ago";
                    }
                    else
                    {
                        if ((int)diff.TotalSeconds < 0)
                            return string.Empty;

                        if ((int)diff.TotalSeconds < 10)
                            return "A moment ago";

                        return string.Format("{0} seconds ago", (int)diff.TotalSeconds);
                    }
                }
                return string.Empty;
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
