using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Streed.Strava.Athletes
{
    [DataContract]
    public class Athlete : INotifyPropertyChanged
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "resource_state")]
        public int ResourceState { get; set; } 

        public bool IsAuthenticatedAthlete { get { return DataAccess.StreedApplicationSettings.AuthenticatedAthleteId == Id; } }

        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }

        [DataMember(Name = "lastname")]
        public string LastName { get; set; }

        [DataMember(Name = "profile_medium")]
        public string MediumProfileUrl { get; set; }

        public BitmapImage MediumProfileImage { get; set; }

        [DataMember(Name = "profile")]
        public string ProfileUrl { get; set; }

        public BitmapImage ProfileImage { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        public string CityAndState
        {
            get
            {
                var city = City;
                var state = State;
                if (string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state)) return string.Empty;
                if (string.IsNullOrWhiteSpace(city)) return state;
                if (string.IsNullOrWhiteSpace(state)) return city;
                if (city.ToUpper().Contains(state.ToUpper()))
                {
                    var index = city.ToUpper().IndexOf(state.ToUpper());
                    city = city.Remove(index);
                }
                return string.Format("{0}, {1}", city, state);
            }
        }

        [DataMember(Name = "premium")]
        public bool Premium { get; set; }

        [DataMember(Name = "follower_count")]
        public int FollowerCount { get; set; }

        [DataMember(Name = "friend_count")]
        public int FriendCount { get; set; }

        public int BothFollowingCount { get; set; }

        [DataMember(Name = "measurement_preference")]
        public string MeasurementPreference { get; set; }

        public MeasurementType MeasurementUnit
        {
            get
            {
                return MeasurementPreference == "feet" ? MeasurementType.Feet : MeasurementType.Meters;
            }
        }

        [DataMember(Name = "friend")]
        public string Friend { get; set; }

        public bool IsFriend
        {
            get
            {
                return (Friend == "accepted" ? true : false);
            }
        }

        [DataMember(Name = "follower")]
        public string Follower { get; set; }

        public bool IsFollowing
        {
            get
            {
                return (Follower == "accepted" ? true : false);
            }
        }

        [DataMember]
        public Athlete[] Friends { get; set; }

        [DataMember]
        public Athlete[] Followers { get; set; }

        [DataMember]
        public Athlete[] BothFollowing { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        public string CreatedAtDate
        {
            get
            {
                return DateTime.Parse(CreatedAt).ToShortDateString();
            }
        }

        [DataMember(Name = "ftp")]
        public int Ftp { get; set; }

        [DataMember(Name = "sex")]
        public string Sex { get; set; }

        public void LoadProfileImages()
        {
            if (MediumProfileImage != null && ProfileImage != null)
                return;

            var repo = new DataAccess.AthleteRepository();

            var ageThreshold = 3;
            if (repo.GetProfileImageAgeInDays(Id) > ageThreshold)
                repo.DeleteProfileImages(Id);

            ProfileImage = repo.GetProfileImage(Id);
            if (ProfileImage == null)
                LoadProfileImageFromNet();

            MediumProfileImage = repo.GetMediumProfileImage(Id);
            if (MediumProfileImage == null)
                LoadMediumProfileImageFromNet();

            NotifyPropertyChanged("MediumProfileImage");
            NotifyPropertyChanged("ProfileImage");
        }

        private void LoadProfileImageFromNet()
        {
            Uri largeUri;
            if (Uri.TryCreate(ProfileUrl, UriKind.Absolute, out largeUri) == false)
            {
                ProfileImage = new BitmapImage(new Uri("/Assets/default-athlete-profile-image.png", UriKind.Relative));
                return;
            }

            ProfileImage = new BitmapImage();
            ProfileImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            ProfileImage.ImageOpened += (o, e) =>
                {
                    if (ProfileImage.UriSource.IsAbsoluteUri)
                    {
                        var repo = new DataAccess.AthleteRepository();
                        repo.InsertProfileImage(Id, ProfileImage);
                    }
                };
            ProfileImage.ImageFailed += (o, e) =>
                { 
                    ProfileImage.UriSource = new Uri("/Assets/default-athlete-profile-image.png", UriKind.Relative);                
                };
            ProfileImage.UriSource = largeUri;
        }

        private void LoadMediumProfileImageFromNet()
        {
            Uri mediumUri;
            if (Uri.TryCreate(MediumProfileUrl, UriKind.Absolute, out mediumUri) == false)
            {
                MediumProfileImage = new BitmapImage(new Uri("/Assets/default-athlete-medium-profile-image.png", UriKind.Relative));
                return;
            }

            MediumProfileImage = new BitmapImage();
            MediumProfileImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            MediumProfileImage.ImageOpened += (o, e) =>
                {
                    if (MediumProfileImage.UriSource.IsAbsoluteUri)
                    {
                        var repo = new DataAccess.AthleteRepository();
                        repo.InsertMediumProfileImage(Id, MediumProfileImage);
                    }
                };
            MediumProfileImage.ImageFailed += (o, e) =>
                {
                    MediumProfileImage.UriSource = new Uri("/Assets/default-athlete-medium-profile-image.png", UriKind.Relative);                
                };
            MediumProfileImage.UriSource = mediumUri;
        }

        public string DebugString()
        {
            return string.Format("Id:{0}\nFullName:{1}\nIsAuthenticatedAthlete:{2}\nMediumProfileUrl:{3}\nProfileUrl:{4}\nResourceState:{5}\n", Id, FullName, IsAuthenticatedAthlete, MediumProfileImage, ProfileUrl, ResourceState);
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
