using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Streed.Strava.Maps
{
    [DataContract]
    public class Map : INotifyPropertyChanged
    {
        private static Size SummaryMapSize = new Size(480.0, 480.0);
        private static Size DetailMapSize = new Size(456.0, 240.0);

        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Seems to be only set for Segments
        /// </summary>
        [DataMember(Name = "polyline")]
        public string Polyline { get; set; }

        /// <summary>
        /// Set for Activities
        /// </summary>
        [DataMember(Name = "summary_polyline")]
        public string SummaryPolyline { get; set; }

        public BitmapImage DetailMap { get; set; }
        public BitmapImage SummaryMap { get; set; }
        public BitmapImage SummaryMapWithPois { get; set; }

        public void LoadDetailMap()
        {
            if (string.IsNullOrEmpty(Polyline))
                return;

            if (DetailMap != null)
                return;

            var repo = new DataAccess.MapRepository();
            DetailMap = repo.GetMap(Id);
            if (DetailMap == null && string.IsNullOrWhiteSpace(Polyline) == false)
            {
                //get Google API Static map with polyline
                var key = DataAccess.StreedApplicationSettings.GoogleApiKey;
                var size = string.Format("{0}x{1}", DetailMapSize.Width, DetailMapSize.Height);
                var polyline = Polyline.Replace(@"\", "%5c");
                var url = GetGoogleStaticMapUrlString(key, size, polyline);

                //MapQuest API static map with polyline and bounding rect
                //var key = DataAccess.StreedApplicationSettings.MapQuestApiKey;
                //var size = string.Format("{0:0},{1:0}", DetailMapSize.Width, DetailMapSize.Height);
                //var geoCoordinates = Strava.Utilities.PolylineToGeoCoordinates(Polyline);
                //var polyline = Polyline.Replace(@"\", "%5c");
                //var boundingRect = Strava.Utilities.BoundingRectangleFromGeoCoordinates(geoCoordinates);
                //var url = GetMapQuestStaticMapUrlString(key, size, polyline, boundingRect, geoCoordinates.First(), geoCoordinates.Last());

                //save/set BitmapImage
                DetailMap = new BitmapImage();
                DetailMap.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                DetailMap.ImageOpened += (o, e) =>
                {
                    repo.InsertMap(Id, DetailMap);
                };
                DetailMap.UriSource = new Uri(url);
#if DEBUG
                System.Diagnostics.Debugger.Log(0, string.Empty, url + Environment.NewLine);
#endif
            }
            NotifyPropertyChanged("DetailMap");
        }

        public void LoadSummaryMaps()
        {
            if (string.IsNullOrEmpty(SummaryPolyline))
                return;

            if (SummaryMap != null && SummaryMapWithPois != null)
                return;

            var repo = new DataAccess.MapRepository();
            SummaryMap = repo.GetSummaryMap(Id);
            if (SummaryMap == null && string.IsNullOrWhiteSpace(SummaryPolyline) == false)
            {
                //get Google API Static map with polyline
                var key = DataAccess.StreedApplicationSettings.GoogleApiKey;
                var size = string.Format("{0}x{1}", SummaryMapSize.Width, SummaryMapSize.Height);
                var polyline = SummaryPolyline.Replace(@"\", "%5c");
                var url = GetGoogleStaticMapUrlString(key, size, polyline);

                //MapQuest API static map with polyline and bounding rect
                //var key = DataAccess.StreedApplicationSettings.MapQuestApiKey;
                //var size = string.Format("{0:0},{1:0}", SummaryMapSize.Width, SummaryMapSize.Height);
                //var geoCoordinates = Strava.Utilities.PolylineToGeoCoordinates(SummaryPolyline);
                //var polyline = SummaryPolyline.Replace(@"\", "%5c");
                //var boundingRect = Strava.Utilities.BoundingRectangleFromGeoCoordinates(geoCoordinates);
                //var url = GetMapQuestStaticMapUrlString(key, size, polyline, boundingRect);

                //save/set BitmapImage
                SummaryMap = new BitmapImage();
                SummaryMap.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                SummaryMap.ImageOpened += (o, e) =>
                    {
                        repo.InsertSummaryMap(Id, SummaryMap);
                    };
                SummaryMap.UriSource = new Uri(url);
#if DEBUG
                System.Diagnostics.Debugger.Log(0, string.Empty, url + Environment.NewLine);                             
#endif
            }
            NotifyPropertyChanged("SummaryMap");

            SummaryMapWithPois = repo.GetSummaryMapWithPois(Id);
            if (SummaryMapWithPois == null && string.IsNullOrWhiteSpace(SummaryPolyline) == false)
            {
                //get Google API Static map with polyline
                var key = DataAccess.StreedApplicationSettings.GoogleApiKey;
                var size = string.Format("{0}x{1}", SummaryMapSize.Width, SummaryMapSize.Height);
                var polyline = SummaryPolyline.Replace(@"\", "%5c");
                var url = GetGoogleStaticMapUrlString(key, size, polyline);

                //MapQuest API static map with polyline, bounding rect, and start/stop pois
                //var key = DataAccess.StreedApplicationSettings.MapQuestApiKey;
                //var size = string.Format("{0:0},{1:0}", SummaryMapSize.Width, SummaryMapSize.Height);
                //var geoCoordinates = Strava.Utilities.PolylineToGeoCoordinates(SummaryPolyline);
                //var polyline = SummaryPolyline.Replace(@"\", "%5c");
                //var boundingRect = Strava.Utilities.BoundingRectangleFromGeoCoordinates(geoCoordinates);
                //var url = GetMapQuestStaticMapUrlString(key, size, polyline, boundingRect, geoCoordinates.First(), geoCoordinates.Last());

                //save/set BitmapImage
                SummaryMapWithPois = new BitmapImage();
                SummaryMapWithPois.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                SummaryMapWithPois.ImageOpened += (o, e) =>
                {
                    repo.InsertSummaryMapWithPois(Id, SummaryMapWithPois);
                };
                SummaryMapWithPois.UriSource = new Uri(url);
#if DEBUG
                System.Diagnostics.Debugger.Log(0, string.Empty, url + Environment.NewLine);
#endif
            }
            NotifyPropertyChanged("SummaryMapWithPois");
        }

        public string GetGoogleStaticMapUrlString(string key, string size, string polyline)
        {
            return string.Format("https://maps.googleapis.com/maps/api/staticmap?key={0}&size={1}&path=weight:3%7Ccolor:red%7Cenc:{2}", key, size, polyline);
        }

        public string GetMapQuestStaticMapUrlString(string key, string size, string polyline, string boundingRectangle, GeoCoordinate start = null, GeoCoordinate end = null)
        {
            var urlFormat = "http://open.mapquestapi.com/staticmap/v4/getmap?key={0}&size={1}&type=map&shapeformat=cmp&shapecolor=0xee1506&shapewidth=5&shape={2}&bestfit={3}&scalebar=false";
            if (start != null && end != null)
            {
                urlFormat += "&pois=red_1,{4},0.5,0.5|green_1,{5},0.5,0.5";
                var scenter = string.Format("{0},{1}", start.Latitude, start.Longitude);
                var ecenter = string.Format("{0},{1}", end.Latitude, end.Longitude);
                return string.Format(urlFormat, key, size, polyline, boundingRectangle, ecenter, scenter);
            }
            return string.Format(urlFormat, key, size, polyline, boundingRectangle);
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
