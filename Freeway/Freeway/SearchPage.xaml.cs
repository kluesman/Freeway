using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;
using Freeway.Classes;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Freeway
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private Polygon SearchPolygonVertical = new Polygon();
        private Polygon SearchPolygonHorizontal = new Polygon();
        private MapLayer mapLayerVertical;
        private MapLayer mapLayerHorizontal;
        private MapOverlay searchOverLayVertical;
        private MapOverlay searchOverLayHorizontal;
        private List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        private List<Address> SearchResults = new List<Address>();
        private GeoCoordinate MyCoordinate = null;
        private GeocodeQuery MyGeocodeQuery = null;
        private ReverseGeocodeQuery MyReverseGeocodeQuery = null;
        private Geolocator MyGeolocator;
        private bool _isRouteSearch = false;
        private bool tracking = false;
        private void LoadSettings()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                tracking = true;
            }
            else
            {
                tracking = ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"]);
            }
            
        }
        public SearchPage()
        {
            InitializeComponent();
            LoadSettings();
            if (tracking)
            {
                InitializeGPS();
            }
            MyReverseGeocodeQuery = new ReverseGeocodeQuery();
            MyReverseGeocodeQuery.QueryCompleted += MyReverseGeocodeQuery_QueryCompleted;
            mapLayerVertical = new MapLayer();
            VerticalMap.Layers.Add(mapLayerVertical);
            mapLayerHorizontal = new MapLayer();
            HorizontalMap.Layers.Add(mapLayerHorizontal);
            LoadSearch();
        }
        private void LoadSearch()
        {
            BitmapImage carBitmap = new BitmapImage();
            carBitmap.UriSource = new Uri(@"/Assets/Images/icons/LocationPin.png", UriKind.RelativeOrAbsolute);
            ImageBrush carBrush = new ImageBrush();
            carBrush.ImageSource = carBitmap;
            carBrush.Stretch = Stretch.UniformToFill;
            carBrush.AlignmentX = AlignmentX.Center;
            carBrush.AlignmentY = AlignmentY.Center;
            SearchPolygonVertical.Points.Add(new Point(-64, -64));
            SearchPolygonVertical.Points.Add(new Point(64, -64));
            SearchPolygonVertical.Points.Add(new Point(64, 64));
            SearchPolygonVertical.Points.Add(new Point(-64, 64));
            SearchPolygonVertical.Fill = carBrush;
            SearchPolygonVertical.Tag = new GeoCoordinate(0, 0);

            SearchPolygonHorizontal.Points.Add(new Point(-64, -64));
            SearchPolygonHorizontal.Points.Add(new Point(64, -64));
            SearchPolygonHorizontal.Points.Add(new Point(64, 64));
            SearchPolygonHorizontal.Points.Add(new Point(-64, 64));
            SearchPolygonHorizontal.Fill = carBrush;
            SearchPolygonHorizontal.Tag = new GeoCoordinate(0, 0);

            searchOverLayVertical = new MapOverlay();
            searchOverLayVertical.Content = SearchPolygonVertical;
            searchOverLayVertical.PositionOrigin = new Point(0.0, 0.0);
            searchOverLayVertical.GeoCoordinate = new GeoCoordinate(0, 0);

            searchOverLayHorizontal = new MapOverlay();
            searchOverLayHorizontal.Content = SearchPolygonHorizontal;
            searchOverLayHorizontal.PositionOrigin = new Point(0.0, 0.0);
            searchOverLayHorizontal.GeoCoordinate = new GeoCoordinate(0, 0);
        }
        private void InitializeGPS()
        {
            // Get the phone's current location.
            MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracy = PositionAccuracy.Default;
            MyGeolocator.MovementThreshold = 20;    // The units are meters.
            MyGeolocator.PositionChanged += MyGeolocator_PositionChanged;

        }
        private void MyGeolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MyCoordinate = new GeoCoordinate();
                MyCoordinate.Longitude = args.Position.Coordinate.Longitude;
                MyCoordinate.Latitude = args.Position.Coordinate.Latitude;
                MyCoordinate.Altitude = args.Position.Coordinate.Altitude ?? 0;
            });
        }

        private void SearchPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            txtSearch.Width = this.ActualWidth - (50 + 8);
            if (e.Orientation == PageOrientation.LandscapeLeft || e.Orientation == PageOrientation.LandscapeRight)
            {
                txtSearch.Width = txtSearch.Width - 50;
                if (e.Orientation == PageOrientation.LandscapeLeft)
                    stackSearch.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                else
                    stackSearch.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                if (HorizontalResult.Items.Count > 0)
                {
                    VerticalSearch.Visibility = System.Windows.Visibility.Collapsed;
                    HorizontalSearch.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (VerticalResult.Items.Count > 0)
                {
                    VerticalSearch.Visibility = System.Windows.Visibility.Visible;
                    HorizontalSearch.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void imgSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SearchQuery();
        }

        private void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.PlatformKeyCode == 13)
            {
                SearchQuery();
            }
        }
        private void SearchQuery()
        {
            MyGeocodeQuery = new GeocodeQuery();
            MyGeocodeQuery.SearchTerm = txtSearch.Text;
            MyGeocodeQuery.GeoCoordinate = MyCoordinate == null ? new GeoCoordinate(0, 0) : MyCoordinate;
            MyGeocodeQuery.QueryCompleted += GeocodeQuery_QueryCompleted;
            MyGeocodeQuery.QueryAsync();
            _isRouteSearch = true;
            VerticalSearch.Visibility = System.Windows.Visibility.Collapsed;
            HorizontalSearch.Visibility = System.Windows.Visibility.Collapsed;
            progressBar.Visibility = System.Windows.Visibility.Visible;
            progressBar.IsIndeterminate = true;
            progressBar.IsEnabled = true;
        }
        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                    if (e.Result.Count > 0)
                    {
                        MyCoordinates.Clear();
                        for (int i = 0; i < e.Result.Count; i++)
                        {
                            MyCoordinates.Add(e.Result[i].GeoCoordinate);
                        }
                        if (MyCoordinates.Count > 0)
                        {
                            SearchResults.Clear();
                            MyReverseGeocodeQuery.GeoCoordinate = MyCoordinates[0];
                            MyReverseGeocodeQuery.QueryAsync();
                        }

                    }
                    else
                    {
                        if (_isRouteSearch) // Query is made to locate the destination of a route
                        {
                            progressBar.Visibility = System.Windows.Visibility.Collapsed;
                            progressBar.IsIndeterminate = false;
                            progressBar.IsEnabled = false;
                        }
                        MessageBox.Show("No match found. Narrow your search e.g. Coffee, Chicago, IL.");
                    }
            }
        }


        void MyReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            MyCoordinates.RemoveAt(0);
            if (e.Error == null)
            {
                for (int i = 0; i < e.Result.Count; i++)
                {
                    SearchResults.Add(new Address(e.Result[i].Information.Address.HouseNumber + " " + e.Result[i].Information.Address.Street, e.Result[i].Information.Address.City + ", " +  e.Result[i].Information.Address.State,e.Result[i].Information.Address.PostalCode, e.Result[i].GeoCoordinate.Longitude,e.Result[i].GeoCoordinate.Latitude,e.Result[i].GeoCoordinate.Altitude));
                }
                if (MyCoordinates.Count==0 &&_isRouteSearch) // Query is made to locate the destination of a route
                {

                    if (!mapLayerHorizontal.Contains(searchOverLayHorizontal))
                        mapLayerHorizontal.Add(searchOverLayHorizontal);
                    if (!mapLayerVertical.Contains(searchOverLayVertical))
                        mapLayerVertical.Add(searchOverLayVertical);
                    VerticalResult.ItemsSource = null;
                    VerticalResult.ItemsSource = SearchResults;
                    HorizontalResult.ItemsSource = null;
                    HorizontalResult.ItemsSource = SearchResults;
                    if (VerticalResult.Items.Count > 0)
                    {
                        VerticalResult.SelectedIndex = 0;
                    }
                    HorizontalResult.SelectedIndex = 0;
                    if (HorizontalResult.Items.Count > 0)
                    if (this.Orientation == PageOrientation.PortraitUp && VerticalResult.Items.Count > 0)
                    {
                        VerticalSearch.Visibility = System.Windows.Visibility.Visible;
                        searchOverLayVertical.GeoCoordinate = VerticalMap.Center;
                    }
                    else
                    {
                        HorizontalSearch.Visibility = System.Windows.Visibility.Visible;
                        searchOverLayHorizontal.GeoCoordinate = HorizontalMap.Center;
                    }
                    progressBar.Visibility = System.Windows.Visibility.Collapsed;
                    progressBar.IsIndeterminate = false;
                    progressBar.IsEnabled = false;
                    _isRouteSearch = false;
                }
            }
            if (MyCoordinates.Count > 0)
            {
                MyReverseGeocodeQuery.GeoCoordinate = MyCoordinates[0];
                MyReverseGeocodeQuery.QueryAsync();
            }
        }

        private void VerticalResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VerticalResult.SelectedItems.Count > 0)
            {
                Address selectedAddress = (Address)VerticalResult.SelectedItem;
                VerticalMap.Center = new GeoCoordinate(selectedAddress.Latitude, selectedAddress.Longitude, selectedAddress.Altitude);
                if (this.Orientation == PageOrientation.Portrait)
                {
                    HorizontalResult.SelectedIndex = VerticalResult.SelectedIndex;
                    VerticalResult.Visibility = System.Windows.Visibility.Visible;
                }

                searchOverLayVertical.GeoCoordinate = VerticalMap.Center;
            }
        }

        private void HorizontalResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HorizontalResult.SelectedItems.Count > 0)
            {
                Address selectedAddress = (Address)HorizontalResult.SelectedItem;
                HorizontalMap.Center = new GeoCoordinate(selectedAddress.Latitude, selectedAddress.Longitude, selectedAddress.Altitude);
                if (this.Orientation != PageOrientation.Portrait)
                {
                    VerticalResult.SelectedIndex = HorizontalResult.SelectedIndex;
                    HorizontalResult.Visibility = System.Windows.Visibility.Visible;
                }
                searchOverLayHorizontal.GeoCoordinate = HorizontalMap.Center;
            }
        }

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
                txtSearch.Width = this.ActualWidth - 50 + 8;
        }

        private void Navigate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Address selectedAddress;
            if (this.Orientation == PageOrientation.PortraitUp)
            {
                selectedAddress = (Address)VerticalResult.SelectedItem;
            }
            else
            {
                selectedAddress = (Address)HorizontalResult.SelectedItem;
            }
            NavigationService.Navigate(new Uri("/MapPage.xaml?Latitude=" + selectedAddress.Latitude.ToString() + "&Longitude=" + selectedAddress.Longitude.ToString() + "&Altitude=" + selectedAddress.Altitude.ToString() , UriKind.Relative));
        }
    }
}