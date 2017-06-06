using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Streed.DataAccess
{
    public class MapRepository
    {
        private readonly string map_filename = "map";
        private readonly string summary_map_filename = "summary_map";
        private readonly string summary_map_with_pois_filename = "summary_map_with_pois";

        public void InsertMap(string mapId, BitmapImage image)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("maps/{0}", mapId));
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, map_filename);

                using (IsolatedStorageFileStream targetStream = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    WriteableBitmap wbm = new WriteableBitmap(image);
                    wbm.SaveJpeg(targetStream, image.PixelWidth, image.PixelHeight, 0, 100);
                }
            }
        }

        public void InsertSummaryMap(string mapId, BitmapImage image)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("maps/{0}", mapId));
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_filename);

                using (IsolatedStorageFileStream targetStream = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    WriteableBitmap wbm = new WriteableBitmap(image);
                    wbm.SaveJpeg(targetStream, image.PixelWidth, image.PixelHeight, 0, 100);
                }
            }
        }

        public void InsertSummaryMapWithPois(string mapId, BitmapImage image)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                userStore.CreateDirectory(string.Format("maps/{0}", mapId));
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_with_pois_filename);

                using (IsolatedStorageFileStream targetStream = userStore.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    WriteableBitmap wbm = new WriteableBitmap(image);
                    wbm.SaveJpeg(targetStream, image.PixelWidth, image.PixelHeight, 0, 100);
                }
            }
        }

        public BitmapImage GetMap(string mapId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, map_filename);
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

        public BitmapImage GetSummaryMap(string mapId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_filename);
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

        public BitmapImage GetSummaryMapWithPois(string mapId)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_with_pois_filename);
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

        public void DeleteMapsLastAccessedInDays(int days)
        {
            using (var userStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (userStore.DirectoryExists("maps"))
                {
                    var searchPath = "maps/*";
                    userStore.GetDirectoryNames(searchPath).ToList().ForEach(mapId =>
                        {
                            try
                            {
                                var filename = string.Format("maps/{0}/{1}.jpg", mapId, map_filename);
                                if (userStore.FileExists(filename))
                                {
                                    var dt = userStore.GetLastAccessTime(filename);
                                    if ((DateTime.Today - dt.Date).Days > days)
                                    {
                                        userStore.DeleteFile(filename);
                                    }
                                }
                                filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_filename);
                                if (userStore.FileExists(filename))
                                {
                                    var dt = userStore.GetLastAccessTime(filename);
                                    if ((DateTime.Today - dt.Date).Days > days)
                                    {
                                        userStore.DeleteFile(filename);
                                    }
                                }
                                filename = string.Format("maps/{0}/{1}.jpg", mapId, summary_map_with_pois_filename);
                                if (userStore.FileExists(filename))
                                {
                                    var dt = userStore.GetLastAccessTime(filename);
                                    if ((DateTime.Today - dt.Date).Days > days)
                                    {
                                        userStore.DeleteFile(filename);
                                    }
                                }
                                if (false == userStore.GetFileNames(string.Format("maps/{0}/*.jpg", mapId)).Any())
                                {
                                    var folder = string.Format("maps/{0}", mapId);
                                    userStore.DeleteDirectory(folder);
                                }
                            }
                            catch (IsolatedStorageException)
                            {
                                //unable to delete file or folder
                            }
                        });
                }
            }
        }
    }
}
