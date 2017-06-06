using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Streed.DataAccess
{
    public class AthleteRepository
    {
        private readonly string ProfileImageFilename = "large.jpg";
        private readonly string MediumProfileImageFilename = "medium.jpg";

        public void InsertProfileImage(long athleteId, BitmapImage image)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, ProfileImageFilename);
            InsertImage(athleteId, filename, image);
        }

        public void InsertMediumProfileImage(long athleteId, BitmapImage image)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, MediumProfileImageFilename);
            InsertImage(athleteId, filename, image);
        }

        private void InsertImage(long athleteId, string filename, BitmapImage image)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("athletes/{0}", athleteId));
                using (IsolatedStorageFileStream targetStream = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    WriteableBitmap wbm = new WriteableBitmap(image);
                    wbm.SaveJpeg(targetStream, image.PixelWidth, image.PixelHeight, 0, 100);
                }
            }
        }

        public BitmapImage GetProfileImage(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, ProfileImageFilename);
            return GetImage(filename);
        }

        public BitmapImage GetMediumProfileImage(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, MediumProfileImageFilename);
            return GetImage(filename);
        }

        private BitmapImage GetImage(string filename)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.FileExists(filename))
                {
                    using (var stream = userStore.OpenFile(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var bm = new BitmapImage();
                        bm.SetSource(stream);
                        return bm;
                    }
                }
            }
            return null;
        }

        public int GetProfileImageAgeInDays(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, ProfileImageFilename);
            return GetImageAgeInDays(filename);
        }

        public int GetMediumProfileImageAgeInDays(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, MediumProfileImageFilename);
            return GetImageAgeInDays(filename);
        }

        private int GetImageAgeInDays(string filename)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("athletes"))
                {
                    if (userStore.FileExists(filename))
                    {
                        var dt = userStore.GetCreationTime(filename);
                        return (DateTime.Today - dt.Date).Days;
                    }
                }
            }
            return int.MinValue;
        }

        public void DeleteProfileImage(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, ProfileImageFilename);
            DeleteFile(filename);
        }

        public void DeleteMediumProfileImage(long athleteId)
        {
            var filename = string.Format("athletes/{0}/{1}", athleteId, MediumProfileImageFilename);
            DeleteFile(filename);
        }

        private void DeleteFile(string filename)
        { 
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.FileExists(filename))
                {
                    userStore.DeleteFile(filename);
                }
            }
        }

        public void DeleteProfileImages(long athleteId)
        {
            DeleteProfileImage(athleteId);
            DeleteMediumProfileImage(athleteId);
        }

        public Strava.Athletes.Athlete GetAthlete(long athleteId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("athletes/{0}/athlete.json", athleteId);
                if (userStore.FileExists(filename))
                {
                    using (var stream = userStore.OpenFile(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Strava.Athletes.Athlete));
                        return (Strava.Athletes.Athlete)serializer.ReadObject(stream);
                    }
                }
            }
            return null;
        }

        public void InsertAthlete(Strava.Athletes.Athlete athlete)
        {
            var existingAthlete = GetAthlete(athlete.Id);
            if (existingAthlete != null)
            {
                //never overwrite a summary/detail athlete with a meta athlete
                if (existingAthlete.ResourceState > athlete.ResourceState &&
                    athlete.ResourceState == 1)
                    return;

                if (existingAthlete.ResourceState > athlete.ResourceState)
                { 
                    //only accept name, profile images, and sex changes
                    //when a summary athlete is trying to overwrite
                    //a detailed athlete
                    existingAthlete.FirstName = athlete.FirstName;
                    existingAthlete.LastName = athlete.LastName;
                    existingAthlete.ProfileUrl = athlete.ProfileUrl;
                    existingAthlete.MediumProfileUrl = athlete.MediumProfileUrl;
                    existingAthlete.Sex = athlete.Sex;

                    athlete = existingAthlete;
                }
            }

            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("athletes/{0}", athlete.Id));
                var filename = string.Format("athletes/{0}/athlete.json", athlete.Id);

                using (var file = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    using (var ms = new MemoryStream())
                    {
                        var serializer = new DataContractJsonSerializer(typeof(Strava.Athletes.Athlete));
                        serializer.WriteObject(ms, athlete);
                        ms.Position = 0;
                        var bytes = ms.ToArray();
                        file.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public void DeleteAllAthletes()
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("athletes"))
                {
                    var athleteIds = userStore.GetDirectoryNames(Path.Combine("athletes", "*"));
                    athleteIds.ToList().ForEach(athleteId =>
                    {
                        var filename = string.Format("athletes/{0}/athlete.json", athleteId);
                        var folder = string.Format("athletes/{0}", athleteId);
                        if (userStore.FileExists(filename))
                        {
                            userStore.DeleteFile(filename);

                            var id = long.Parse(athleteId);
                            DeleteProfileImages(id);

                            try
                            {
                                userStore.DeleteDirectory(folder);
                            }
                            catch { }
                        }
                    });
                }
            }
        }
    }
}
