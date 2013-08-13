using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FreeWay101.Resources;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Device.Location;
using Windows.Phone.Speech.Synthesis;
using Freeway.Classes;

namespace FreeWay101
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mainMap.Center = new System.Device.Location.GeoCoordinate(41.532887, -87.355024);
            mainMap.ZoomLevel = 17;
            mainMap.Pitch = 45;
            mainMap.LandmarksEnabled = true;
            InitializeGPS();
            InitializePolygons();
            mapLayer = new MapLayer();
            mapLayer.Add(Car);
            mainMap.Layers.Add(mapLayer);
            //--Add to load

            routeQuery = new RouteQuery();
            routeQuery.QueryCompleted += routeQuery_QueryCompleted;

        }
        Geolocator myPosition;
        private void InitializeGPS()
        {
            myPosition = new Geolocator();
            myPosition.DesiredAccuracyInMeters = 15;
            myPosition.MovementThreshold = 10;    // The units are meters.
            myPosition.PositionChanged += myPosition_Changed;
        }

        private void myPosition_Changed(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(()=>
            {
                try
                {
                    double CurrentSpeed = (double)args.Position.Coordinate.Speed;
                    double CurrentHeading = (double)args.Position.Coordinate.Heading;
                    string heading = CurrentHeading.ToString();
                    CurrentHeading = heading == "NaN" ? 0 : CurrentHeading;
                    mainMap.Heading = CurrentHeading;
                    mainMap.Center.Longitude = args.Position.Coordinate.Longitude;
                    mainMap.Center.Latitude = args.Position.Coordinate.Latitude;
                    mainMap.Center.Altitude = (double)args.Position.Coordinate.Altitude;
                    MyCoordinate.Longitude = mainMap.Center.Longitude;
                    MyCoordinate.Latitude = mainMap.Center.Latitude;
                    MyCoordinate.Altitude = mainMap.Center.Altitude;
                    mainMap.ZoomLevel = 13;
                    if (Car.Content == null)
                    {
                        Car.Content = CarPolygon;
                    }
                    Car.GeoCoordinate = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);

                    if (routepoints!= null && routepoints.Count > 0)
                    {
                        double distMeters = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, routepoints[0].Geo_Coordinate.Longitude, routepoints[0].Geo_Coordinate.Latitude, GeoMath.MeasureUnits.Kilometers);
                        distMeters = distMeters * 1000;
                    }
                }
                catch (Exception ex)
                {
                }
            });  
        }
        RouteQuery routeQuery;
        Route route;
        Polygon CarPolygon = new Polygon();
        Polygon DestinationPolygon = new Polygon();
        MapLayer mapLayer;
        MapOverlay Car, Destination;
        MapRoute MyMapRoute;
		GeoCoordinate MyCoordinate = new GeoCoordinate();
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        private void InitializePolygons()
        {
            BitmapImage polygonBitmap = new BitmapImage();
            polygonBitmap.UriSource = new Uri(@"/Assets/Images/Vehicals/fast.png", UriKind.RelativeOrAbsolute);
            ImageBrush polygonBrush = new ImageBrush();
            polygonBrush.ImageSource = polygonBitmap;
            polygonBrush.Stretch = Stretch.UniformToFill;
            polygonBrush.AlignmentX = AlignmentX.Center;
            polygonBrush.AlignmentY = AlignmentY.Center;
            CarPolygon.Points.Add(new Point(-50, -50));
            CarPolygon.Points.Add(new Point(50, -50));
            CarPolygon.Points.Add(new Point(50, 50));
            CarPolygon.Points.Add(new Point(-50, 50));
            CarPolygon.Fill = polygonBrush;
            CarPolygon.Tag = new GeoCoordinate(0, 0);
            Car = new MapOverlay();
            Car.Content = CarPolygon;
            Car.PositionOrigin = new Point(0.0, 0.0);
            Car.GeoCoordinate = new GeoCoordinate(0, 0);
            polygonBitmap = new BitmapImage();
            polygonBitmap.UriSource = new Uri(@"/Assets/Images/Finish.png", UriKind.RelativeOrAbsolute);
            polygonBrush = new ImageBrush();
            polygonBrush.ImageSource = polygonBitmap;
            polygonBrush.Stretch = Stretch.UniformToFill;
            polygonBrush.AlignmentX = AlignmentX.Center;
            polygonBrush.AlignmentY = AlignmentY.Center;
            DestinationPolygon.Points.Add(new Point(-64, -64));
            DestinationPolygon.Points.Add(new Point(64, -64));
            DestinationPolygon.Points.Add(new Point(64, 64));
            DestinationPolygon.Points.Add(new Point(-64, 64));
            DestinationPolygon.Fill = polygonBrush;
            DestinationPolygon.Tag = new GeoCoordinate(0, 0);
            Destination = new MapOverlay();
            Destination.Content = CarPolygon;
            Destination.PositionOrigin = new Point(0.0, 0.0);
            Destination.GeoCoordinate = new GeoCoordinate(0, 0);

            //Add Car MapOverlay to Load
        }
        GeocodeQuery MyGeocodeQuery = new GeocodeQuery();
        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            MyGeocodeQuery = new GeocodeQuery();
            MyGeocodeQuery.SearchTerm = "800 East Grand Avenue, Chicago, IL 60611";
            MyGeocodeQuery.GeoCoordinate = MyCoordinate == null ? new GeoCoordinate(0, 0) : MyCoordinate;
            MyGeocodeQuery.QueryCompleted += GeocodeQuery_QueryCompleted;
            MyGeocodeQuery.QueryAsync();
        }

        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MyCoordinates.Clear();
                    MyCoordinates = (from p in e.Result select p.GeoCoordinate).ToList<GeoCoordinate>();
                        if (MyCoordinates.Count > 0)
                        {
                            List<GeoCoordinate> routeWaypoints = new List<GeoCoordinate>();
                            routeWaypoints.Add(new GeoCoordinate(MyCoordinates[0].Latitude, MyCoordinates[0].Longitude));
                            routeWaypoints.Add(MyCoordinate);
                            routeQuery.Waypoints = routeWaypoints;
                            routeQuery.QueryAsync();
                        }
                }
                else
                {
                    MessageBox.Show("You don't know how to type");
                }
            }
        }
        List<RoutePoint> routepoints;
        class RoutePoint
        {
            public RoutePoint(GeoCoordinate point, string instruct)
            {
                GeoCoordinate Geo_Coordinate = point;
                instruction = instruct;
                hasArrived = false;
                hasSpoken = false;
            }
            public GeoCoordinate Geo_Coordinate { get; set; }
            public string instruction { get; set; }
            public bool hasSpoken { get; set; }
            public bool hasArrived { get; set; }
        }
        private void routeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {

            if (e.Error == null)
            {
                if (e.Result.Legs.Count == 0)
                    return;
                route = e.Result;
                routepoints = (from p in route.Legs[0].Maneuvers select new RoutePoint(p.StartGeoCoordinate, p.InstructionText)).ToList();
                if (MyMapRoute != null)
                    mainMap.RemoveRoute(MyMapRoute);
                MyMapRoute = new MapRoute(route);
                mainMap.AddRoute(MyMapRoute);
                speak(route.Legs[0].Maneuvers[0].InstructionText);
            }
        }
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private async void speak(string speechtext)
        {
            try
            {
                await synthesizer.SpeakTextAsync(speechtext);
            }
            catch
            {
            }
        }
    }
}