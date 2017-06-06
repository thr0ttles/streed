using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Streed.Pages
{
    public partial class MapPage : PhoneApplicationPage
    {
        private string MapPolyline { get; set; }
        private IEnumerable<System.Device.Location.GeoCoordinate> Coordinates { get; set; }
        private bool SetViewAfterOnLoaded { get; set; }

        public MapPage()
        {
            InitializeComponent();

            Map.Loaded += Map_Loaded;
            Map.Unloaded += Map_Unloaded;
            Map.ViewChanged += Map_ViewChanged;
            Map.ResolveCompleted += Map_ResolveCompleted;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string polyline;
            if (this.NavigationContext.QueryString.TryGetValue("polyline", out polyline))
            {
                MapPolyline = polyline.Replace("%5c", @"\");
                Coordinates = Strava.Utilities.PolylineToGeoCoordinates(MapPolyline);
            }
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            SetViewAfterOnLoaded = true;

            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = DataAccess.StreedApplicationSettings.MapApplicationId;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = DataAccess.StreedApplicationSettings.MapAuthenticationToken;
        }

        void Map_Unloaded(object sender, RoutedEventArgs e)
        {
            Map.ViewChanged -= Map_ViewChanged;
            Map.ResolveCompleted -= Map_ResolveCompleted;
        }

        void Map_ViewChanged(object sender, MapViewChangedEventArgs e)
        {
            if (Coordinates != null && Coordinates.Count() != 0)
            {
                var mp = new MapPolyline
                {
                    StrokeColor = Color.FromArgb(255, 255, 0, 0),
                    StrokeThickness = 6
                };
                Coordinates.ToList().ForEach(c => mp.Path.Add(c));
                Map.MapElements.Add(mp);

                var startPin = new MapOverlay();
                startPin.PositionOrigin = new Point(0.5, 0.5);
                startPin.Content = new Image
                    {
                        Source = new BitmapImage(new Uri("/Assets/map-icon-start.png", UriKind.Relative))
                    };
                startPin.GeoCoordinate = Coordinates.ElementAt(0);

                var endPin = new MapOverlay();
                endPin.PositionOrigin = new Point(0.5, 0.5);
                endPin.Content = new Image 
                    { 
                        Source = new BitmapImage(new Uri("/Assets/map-icon-end.png", UriKind.Relative))
                    };
                endPin.GeoCoordinate = Coordinates.ElementAt(Coordinates.Count() - 1);

                var ml = new MapLayer();
                ml.Add(endPin);
                ml.Add(startPin);

                Map.Layers.Add(ml);
            }
        }

        void Map_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            if (SetViewAfterOnLoaded && Coordinates != null && Coordinates.Count() != 0)
            {
                SetViewAfterOnLoaded = false;
                var boundingRectangle = LocationRectangle.CreateBoundingRectangle(Coordinates);
                Map.SetView(boundingRectangle, MapAnimationKind.Parabolic);
            }
        }

        private void LayersButton_Click(object sender, RoutedEventArgs e)
        {
            ContentPanel.Opacity = .75;
            LayersPopupGrid.Width = App.Current.Host.Content.ActualWidth;
            LayersPopupGrid.Height = App.Current.Host.Content.ActualHeight;
            switch (Map.CartographicMode)
            { 
                case MapCartographicMode.Road:
                    RoadButton.IsChecked = true;
                    break;
                case MapCartographicMode.Aerial:
                    AerialButton.IsChecked = true;
                    break;
                case MapCartographicMode.Hybrid:
                    HybridButton.IsChecked = true;
                    break;
                case MapCartographicMode.Terrain:
                    TerrainButton.IsChecked = true;
                    break;
            }
            LayersPopup.IsOpen = true;
        }

        private void RoadButton_Click(object sender, RoutedEventArgs e)
        {
            Map.CartographicMode = MapCartographicMode.Road;
            LayersPopup.IsOpen = false;
            ContentPanel.Opacity = 1;
        }

        private void AerialButton_Click(object sender, RoutedEventArgs e)
        {
            Map.CartographicMode = MapCartographicMode.Aerial;
            LayersPopup.IsOpen = false;
            ContentPanel.Opacity = 1;
        }

        private void HybridButton_Click(object sender, RoutedEventArgs e)
        {
            Map.CartographicMode = MapCartographicMode.Hybrid;
            LayersPopup.IsOpen = false;
            ContentPanel.Opacity = 1;
        }

        private void TerrainButton_Click(object sender, RoutedEventArgs e)
        {
            Map.CartographicMode = MapCartographicMode.Terrain;
            LayersPopup.IsOpen = false;
            ContentPanel.Opacity = 1;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}