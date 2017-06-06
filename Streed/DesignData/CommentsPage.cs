using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streed.DesignData
{
    public class CommentsPage
    {
        public List<Strava.Comments.Comment> Comments { get; set; }
        public string ActivityName { get; set; }

        public CommentsPage()
        {
            ActivityName = "Saw the sun today, Briefly";
            Comments = new List<Strava.Comments.Comment>();

            Comments.Add(new Strava.Comments.Comment
                {
                    ActivityId = 1,
                    Athlete = new Strava.Athletes.Athlete
                    {
                        FirstName = "Jim",
                        LastName = "Sweet",
                        Id = 1,
                        MediumProfileUrl = "",
                        MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                        ProfileUrl = ""
                    },
                    CreatedDateTime = "2014-12-19T15:55:23Z",
                    Id = 1,
                    Text = "roll off? did ya launch the bike?"
                });

            Comments.Add(new Strava.Comments.Comment
            {
                ActivityId = 1,
                Athlete = new Strava.Athletes.Athlete
                {
                    FirstName = "Neal",
                    LastName = "Wilson",
                    Id = 1,
                    MediumProfileUrl = "",
                    MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                    ProfileUrl = ""
                },
                CreatedDateTime = "2012-03-07T00:13:35Z",
                Id = 1,
                Text = "yea dent sound good tell the whole story buddy! hope you're ok and you didn't damage the bike like last year!"
            });

            Comments.Add(new Strava.Comments.Comment
            {
                ActivityId = 1,
                Athlete = new Strava.Athletes.Athlete
                {
                    FirstName = "Gary",
                    LastName = "K",
                    Id = 1,
                    MediumProfileUrl = "",
                    MediumProfileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/my.profile.medium.jpg", UriKind.Relative)),
                    ProfileUrl = ""
                },
                CreatedDateTime = "2012-03-07T00:12:35Z",
                Id = 1,
                Text = "I was sprinting leaned too far forward and bike lundged forward off the rollers i was able to clip out fast with the spd MTB pedals, luckily did not fall,, rear wheel ended up hitting front roller, this is even with the horizontal motion that i had built.   Too much weight on the front wheel, too little weight on the rear wheel. add 50 watts to the value for almost accurate power with the power cal heart rate power meter"
            });
        }
    }
}
