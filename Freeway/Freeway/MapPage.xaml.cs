using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
using Windows.Phone.Speech.Synthesis; 
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System.IO.IsolatedStorage;
using System.Linq;
using Freeway.Classes;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Freeway
{
    public partial class MapPage : PhoneApplicationPage
    {
        class RoutePoint
        {
            public RoutePoint(GeoCoordinate point, string instruct)
            {GeoCoordinate
                Geo_Coordinate = point;
                instruction = instruct;
                hasArrived = false;
                hasSpoken = false;
            }
            public GeoCoordinate Geo_Coordinate {get;set;}
            public string instruction { get; set; }
            public bool hasSpoken { get; set; }
            public bool hasArrived { get; set; }
        }
        #region LocalVariables
        SpeechSynthesizer synthesizer; 
        Address RouteAddress;
        GPSHelper gpshelper;
        Geolocator MyGeolocator;
        GeoCoordinate RouteLocation;
        ReverseGeocodeQuery reverseQuery;
        RouteQuery routeQuery;
        Route route;
        //Compass compass;
        int FirstLoad = 0;
        Polygon CarPolygon = new Polygon();
        Polygon DestinationPolygon = new Polygon();
        MapLayer mapLayer;
        MapOverlay Car, Destination;
        MapRoute MyMapRoute;
        List<RoutePoint> routepoints;
        bool tracking = false;
        bool ScreenLockOff = true;
        bool Enabled3D = true;
        bool Buildings = true;
        bool DayNightEnabled = true;
        bool SmartZoomEnabled = false;
        bool BearingNorth = false;
        bool isCentered = true;
        string VehicalChoice = "";
        double dHeading = 0.00;
        int offCourseRange = 30, NotifyFarRange = 40, NotifyCloseRange = 20, currentPoint;
        #endregion
        // Constructor.
        public MapPage()
        {
            InitializeComponent();
            StartMap();
            routeQuery = new RouteQuery();
            routeQuery.QueryCompleted += routeQuery_QueryCompleted;
            CarPolygon.Tap += CarPolygon_Tap;

        }
        #region Polygons
        //such as cars and flags
        private void LoadPolygons()
        {
            BitmapImage polygonBitmap = new BitmapImage();
            polygonBitmap.UriSource = new Uri(@"/Assets/Images/Vehicals/7.png", UriKind.RelativeOrAbsolute);
            //polygonBitmap.UriSource = new Uri(@"/Assets/Images/Vehicals/" + VehicalChoice + ".png", UriKind.RelativeOrAbsolute);
            ImageBrush polygonBrush = new ImageBrush();
            polygonBrush.ImageSource = polygonBitmap;
            polygonBrush.Stretch = Stretch.UniformToFill;
            polygonBrush.AlignmentX = AlignmentX.Center;
            polygonBrush.AlignmentY = AlignmentY.Center;
            CarPolygon.Points.Add(new Point(-64, -64));
            CarPolygon.Points.Add(new Point(64, -64));
            CarPolygon.Points.Add(new Point(64, 64));
            CarPolygon.Points.Add(new Point(-64, 64));
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
        }
        void CarPolygon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isCentered == false)
            {
                MyMap.Center = Car.GeoCoordinate;
                isCentered = true;
            }
        }
        #endregion
        #region LoadSettings
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
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("Enabled3D"))
            {
                IsolatedStorageSettings.ApplicationSettings["Enabled3D"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                Enabled3D = true;
            }
            else
            {
                Enabled3D = ((bool)IsolatedStorageSettings.ApplicationSettings["Enabled3D"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("Buildings"))
            {
                IsolatedStorageSettings.ApplicationSettings["Buildings"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                Buildings = true;
            }
            else
            {
                Buildings = ((bool)IsolatedStorageSettings.ApplicationSettings["Buildings"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("DayNightEnabled"))
            {
                IsolatedStorageSettings.ApplicationSettings["DayNightEnabled"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                DayNightEnabled = true;
            }
            else
            {
                DayNightEnabled = ((bool)IsolatedStorageSettings.ApplicationSettings["DayNightEnabled"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("SmartZoomEnabled"))
            {
                IsolatedStorageSettings.ApplicationSettings["SmartZoomEnabled"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                SmartZoomEnabled = true;
            }
            else
            {
                SmartZoomEnabled = ((bool)IsolatedStorageSettings.ApplicationSettings["SmartZoomEnabled"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("BearingNorth"))
            {
                IsolatedStorageSettings.ApplicationSettings["BearingNorth"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                BearingNorth = true;
            }
            else
            {
                BearingNorth = ((bool)IsolatedStorageSettings.ApplicationSettings["BearingNorth"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("VehicalChoice"))
            {
                IsolatedStorageSettings.ApplicationSettings["VehicalChoice"] = "35";
                IsolatedStorageSettings.ApplicationSettings.Save();
                VehicalChoice = "35";
            }
            else
            {
                VehicalChoice = ((string)IsolatedStorageSettings.ApplicationSettings["VehicalChoice"]);
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("ScreenLockOff"))
            {
                IsolatedStorageSettings.ApplicationSettings["ScreenLockOff"] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                ScreenLockOff = true;
            }
            else
            {
                ScreenLockOff = ((bool)IsolatedStorageSettings.ApplicationSettings["ScreenLockOff"]);
            }
            if(ScreenLockOff)
                Microsoft.Phone.Shell.PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Disabled;
            else
                Microsoft.Phone.Shell.PhoneApplicationService.Current.UserIdleDetectionMode = Microsoft.Phone.Shell.IdleDetectionMode.Enabled;
        }
        #endregion
        private void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
            mapLayer = new MapLayer();
            if (tracking)
            {
                Car.Content = null;
                mapLayer.Add(Car);
            }
            if (RouteAddress != null)
            {
                Destination.Content = DestinationPolygon;
                mapLayer.Add(Destination);
            }
            MyMap.Layers.Add(mapLayer);
            try
            {
                this.synthesizer = new SpeechSynthesizer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize the synthesizer: " + ex.Message);
            }
        }

        // When the page is loaded, make sure that you have obtained the users consent to use their location
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string sLatitude = "0.0";
            string sLongitude = "0.0";
            string sName = "";
            if (NavigationContext.QueryString.TryGetValue("Latitude", out sLatitude))
            {
            }
            if (NavigationContext.QueryString.TryGetValue("Longitude", out sLongitude))
            {
            }
            if (NavigationContext.QueryString.TryGetValue("Name", out sName))
            {
            }
            if ((sName + sLongitude + sLatitude) != "")
            {
                RouteLocation = new GeoCoordinate();
                RouteLocation.Latitude = double.Parse(sLatitude);
                RouteLocation.Longitude = double.Parse(sLongitude);
                RouteLocation.Altitude = 0.00;
                RouteAddress = new Address();
                RouteAddress.Latitude = RouteLocation.Latitude;
                RouteAddress.Longitude = RouteLocation.Longitude;
                RouteAddress.Address1 = sName;
                Destination.GeoCoordinate = RouteLocation;
                reverseQuery = new ReverseGeocodeQuery();
                reverseQuery.GeoCoordinate = RouteLocation;
                reverseQuery.QueryCompleted += reverseQuery_QueryCompleted;
                reverseQuery.QueryAsync();
                currentPoint = 0;
            }
        }

        void reverseQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                RouteAddress.Address2 = e.Result[0].Information.Address.HouseNumber + " " + e.Result[0].Information.Address.Street + ", "+ 
                                        e.Result[0].Information.Address.City + ", " + e.Result[0].Information.Address.State;
                RouteAddress.Address3 = e.Result[0].Information.Address.PostalCode;
            }
        }

        private void StartMap()
        {
            LoadSettings();
            MyMap.ZoomLevel = 2;
            gpshelper = new GPSHelper();// A nullable int
            mapLayer = new MapLayer();
            LoadPolygons();
            if (BearingNorth)
            {
                //if (Compass.IsSupported)
                //{
                //    IsCompassSupported = true;
                //    compass = new Compass();
                //    compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(500);  // Must be multiple of 20
                //    compass.Start();
                //}
            }
            if (tracking)
            {

                if (Buildings)
                {
                    MyMap.LandmarksEnabled = true;
                    MyMap.PedestrianFeaturesEnabled = true;
                }
                if (Enabled3D)
                {
                    MyMap.Pitch = 45;
                    //MyMap.Height = this.ActualHeight * 2;
                    //MyMap.Margin = new Thickness(0, this.ActualHeight / 2, 0, 0);
                }

                /* Zoom levels
                    Level Name 
                    14 Street 
                    11 City 
                    8 County 
                    3 Country 
                 */
                InitializeGPS();
            }

        }
        private void InitializeGPS()
        {
            // Get the phone's current location.
            //MyGeolocator.DesiredAccuracy = PositionAccuracy.High;
            MyGeolocator = new Geolocator();
            MyGeolocator.DesiredAccuracyInMeters = 5;
            MyGeolocator.MovementThreshold = 10;    // The units are meters.
            MyGeolocator.PositionChanged += MyGeolocator_PositionChanged;

        }

        private void MyGeolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (FirstLoad == 0 &&Enabled3D)
                {
                    MyMap.Pitch = 45;
                    MyMap.Height = this.ActualHeight * 1.4;
                    MyMap.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    FirstLoad++;
                }
                MyMap.ZoomLevel = MyMap.ZoomLevel == 2 ? 
                                    18
                                    : MyMap.ZoomLevel;
                if (DayNightEnabled)
                {
                    if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 18)
                    {
                        if (MyMap.ColorMode != MapColorMode.Light)
                            MyMap.ColorMode = MapColorMode.Light;
                    }
                    else
                    {
                        if (MyMap.ColorMode != MapColorMode.Dark)
                            MyMap.ColorMode = MapColorMode.Dark;
                    }
                }
                double currentSpeed = args.Position.Coordinate.Speed != null ? gpshelper.MilesPerHour(Convert.ToDouble(args.Position.Coordinate.Speed)) : 0;
                if (SmartZoomEnabled && isCentered)
                {
                    if (currentSpeed > 60)
                    {
                        offCourseRange = 150;
                        if (MyMap.ZoomLevel != 15)
                            MyMap.ZoomLevel = 15;
                    }
                    else if (currentSpeed > 40 && currentSpeed <= 60)
                    {
                        offCourseRange = 45;
                        if (MyMap.ZoomLevel != 17)
                            MyMap.ZoomLevel = 17;
                    }
                    else if (currentSpeed > 0 && currentSpeed <= 40)
                    {
                        offCourseRange = 20;
                        if (MyMap.ZoomLevel != 18)
                            MyMap.ZoomLevel = 18;
                    }

                    if ((Enabled3D) && (currentSpeed > 60))
                    {
                        MyMap.Pitch = 38;
                    }
                    else
                    {
                        MyMap.Pitch = 45;
                    }
                }
                if (BearingNorth && currentSpeed>5)
                {
                    //double CurrentHeading = IsCompassSupported ? compass.CurrentValue.MagneticHeading : (double)args.Position.Coordinate.Heading;
                    double CurrentHeading = (double)args.Position.Coordinate.Heading;
                    string heading = CurrentHeading.ToString();
                    CurrentHeading = heading == "NaN" ? 0 : CurrentHeading;
                    MyMap.Heading = CurrentHeading;
                }
                if(isCentered)
                    MyMap.Center = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
                if (Car.Content == null)
                {
                    Car.Content = CarPolygon;
                    if (RouteAddress != null && routeQuery.Waypoints == null)
                    {
                        List<GeoCoordinate> routeWaypoints = new List<GeoCoordinate>();
                        routeWaypoints.Add(new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude));
                        routeWaypoints.Add(RouteLocation);
                        routeQuery.Waypoints = routeWaypoints;
                        routeQuery.QueryAsync();
                    }
                }
                if (routeQuery.Waypoints != null && route!=null)
                {
                    checkIfOffRoute();
                    speakNearestPoint();
                }
                Car.GeoCoordinate = new GeoCoordinate(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
            });
        }

        private void routeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
             {
                if(e.Result.Legs.Count==0)
                    return;
                route = e.Result;
                routepoints = (from p in route.Legs[0].Maneuvers select new RoutePoint(p.StartGeoCoordinate, p.InstructionText)).ToList();
                if(MyMapRoute!=null)
                    MyMap.RemoveRoute(MyMapRoute);
                MyMapRoute = new MapRoute(route);
                MyMap.AddRoute(MyMapRoute);
             }
        }

        private void NavDirections_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideNavBar();
        }

        private void MapPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            NavDirections.Visibility = Visibility.Collapsed;
            if (Enabled3D)
            {
                MyMap.Pitch = 45;
                MyMap.Height = this.ActualHeight * 1.5;
                MyMap.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            }
            if(e.Orientation== PageOrientation.LandscapeLeft || e.Orientation== PageOrientation.LandscapeRight)
                NavDirections.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            else
                NavDirections.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        }

        private void ShowNavBar()
        {
            NavDirections.Visibility = Visibility.Visible;
            //if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            //{
            //    NavDirections.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            //    NavDirections.Margin = new Thickness(0, 0, 0, 0);
            //    LandscapeFloatDown.Begin();
            //}
            //else
            //{
                NavDirections.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                NavDirections.Margin = new Thickness(0, 0, 0, -100);
                FloatUp.Begin();
            //}
        }

        private void HideNavBar()
        {
            if (NavDirections.Visibility == System.Windows.Visibility.Visible)
            {
                //if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
                //{
                //    LandscapeFloatUp.Begin();
                //}
                //else
                //{
                    FloatDown.Begin();
                //}
                //NavDirections.Visibility = System.Windows.Visibility.Collapsed;
            }
            NavDirections.Visibility = Visibility.Collapsed;
        }

        private string fixDirections(string direction)
        {
            string result = "";
            if ("us" == "us")
            {
                result = direction.Replace(" E.", " East ").Replace(" W ", " West ").
                         Replace(" N.", " North ").Replace(" S ", " South ").
                         Replace(" Ave.", " Avenue ").Replace(" Dr.", " Drive ").
                         Replace(" St.", " Street ").Replace(" Rd.", " Road ").
                         Replace(" Ln.", " Lane ").Replace(" I-", " Interstate ").
                         Replace("SR-", "State Road ").Replace("CR-", "Country Road ").
                         Replace(" Expy ", " Express way ").
                         Replace("-", "").Replace("/", "");
            }
            return result;
        }

        private void MyMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isCentered = false;
        }
        private void checkIfOffRoute()
        {
            if (route.Legs.Count == 0)
                return;
            if (route.Legs[0].Geometry.Count == 0)
                return;
            int loc1 = 0, loc2 = 0;
            GeoCoordinate geoCord1, geoCord2;
            geoCord1 = new GeoCoordinate();
            geoCord2 = new GeoCoordinate();
            double dist1 = 9999999999999999999, dist2 = 9999999999999999999;
            for (int i = 0; i < route.Legs[0].Geometry.Count; i++)
            {
                Double tDist = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, route.Legs[0].Geometry[i].Longitude, route.Legs[0].Geometry[i].Latitude, GeoMath.MeasureUnits.Kilometers);
                tDist = tDist * 1000;
                if (tDist < dist1)
                {
                    dist1 = tDist;
                    loc1 = i;
                    geoCord1 = new GeoCoordinate();
                    geoCord1 = route.Legs[0].Geometry[i];
                }
            }
            if (loc1 >= 0 && route.Legs[0].Geometry.Count >= 2)
            {
                Double tDist, tDist1;
                if (loc1 > 0)
                {
                    tDist = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, route.Legs[0].Geometry[loc1 + 1].Longitude, route.Legs[0].Geometry[loc1 + 1].Latitude, GeoMath.MeasureUnits.Kilometers);
                    tDist = tDist * 1000;
                    tDist1 = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, route.Legs[0].Geometry[loc1 - 1].Longitude, route.Legs[0].Geometry[loc1 - 1].Latitude, GeoMath.MeasureUnits.Kilometers);
                    tDist1 = tDist1 * 1000;
                    geoCord2 = new GeoCoordinate();
                    if (tDist < tDist1)
                    {
                        geoCord2 = route.Legs[0].Geometry[loc1 + 1];
                    }
                    else
                    {
                        geoCord2 = route.Legs[0].Geometry[loc1 + 1];
                    }
                }
                else
                {
                    geoCord2 = new GeoCoordinate();
                    geoCord2 = route.Legs[0].Geometry[loc1 + 1];
                }
            }
            else if (loc1 >= 0 && route.Legs[0].Geometry.Count < 2)
            {
                loc2 = loc1;
                geoCord2 = new GeoCoordinate();
                geoCord2 = route.Legs[0].Geometry[loc2];
            }
            else
            {
                return;
            }

            double r_numerator = (Car.GeoCoordinate.Longitude - geoCord1.Longitude) * (geoCord2.Longitude - geoCord1.Longitude) + (Car.GeoCoordinate.Latitude - geoCord1.Latitude) * (geoCord2.Latitude - geoCord1.Latitude);
            double r_denomenator = (geoCord2.Longitude - geoCord1.Longitude) * (geoCord2.Longitude - geoCord1.Longitude) + (geoCord2.Latitude - geoCord1.Latitude) * (geoCord2.Latitude - geoCord1.Latitude);
            double r = r_numerator / r_denomenator;
            //
            double px = geoCord1.Longitude + r * (geoCord2.Longitude - geoCord1.Longitude);
            double py = geoCord1.Latitude + r * (geoCord2.Latitude - geoCord1.Latitude);
            double distFromNearest = 0.00;
            distFromNearest = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, px, py, GeoMath.MeasureUnits.Kilometers);
            distFromNearest = (distFromNearest * 1000);
            if (distFromNearest >= offCourseRange)
            {
                //dirTurns.lstDirections.Items.Clear();
                //Pushpin Destination = new Pushpin();
                //Destination.Location = polRoute.Locations[polRoute.Locations.Count - 1];
                //polRoute = new MapPolyline();
                //dirTurns.bReroute = true;
                //currentPoint = 0;
                //dirTurns.getRoute(pinCurrentLocation, Destination);
                currentPoint = 0;
                if (routeQuery != null)
                {
                    try
                    {
                        if (routeQuery.IsBusy == false)
                        {
                            if (MyMapRoute != null)
                            {
                                MyMap.RemoveRoute(MyMapRoute);
                            }
                            MyMapRoute = null;
                            List<GeoCoordinate> routeWaypoints = new List<GeoCoordinate>();
                            routeWaypoints.Add(new GeoCoordinate(Car.GeoCoordinate.Latitude, Car.GeoCoordinate.Longitude));
                            routeWaypoints.Add(RouteLocation);
                            routeQuery.Waypoints = routeWaypoints;
                            routeQuery.QueryAsync();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public async void speakNearestPoint()
        {
            GeoCoordinate geoCord = new GeoCoordinate();
            Double MinDistance = 9999999999999;
            int MinIndex = 0;
            double distMeters = GeoMath.Distance(Car.GeoCoordinate.Longitude, Car.GeoCoordinate.Latitude, routepoints[currentPoint].Geo_Coordinate.Longitude, routepoints[currentPoint].Geo_Coordinate.Latitude, GeoMath.MeasureUnits.Kilometers);
            distMeters = distMeters * 1000;
            if (distMeters <= 3000 && distMeters > NotifyFarRange &&  routepoints[currentPoint].hasSpoken == false)
            {
                 routepoints[currentPoint].hasSpoken = true;
                try
                {
                    await this.synthesizer.SpeakTextAsync(fixDirections(routepoints[currentPoint].instruction));
                }
                catch
                {
                    // Ignore the exception which may be generated if the synthesizer is already in the middle of saying something 
                }

                //if (MinIndex == routepoints.Count- 1)
                //{
                //    try
                //    {
                //        await this.synthesizer.SpeakTextAsync("You have arrived at your destination");
                //    }
                //    catch
                //    {
                //        // Ignore the exception which may be generated if the synthesizer is already in the middle of saying something 
                //    } 
                //}
            }
            else if (distMeters >= 0 && distMeters <= NotifyCloseRange && routepoints[currentPoint].hasArrived == false)
            {
                routepoints[currentPoint].hasArrived = true;
                
                try
                {
                    await this.synthesizer.SpeakTextAsync(fixDirections(routepoints[currentPoint].instruction));
                }
                catch
                {
                    // Ignore the exception which may be generated if the synthesizer is already in the middle of saying something 
                }
                //if (MinIndex < routepoints.Count - 1)
                //{
                //    try
                //    {
                //        await this.synthesizer.SpeakTextAsync("You have arrived at your destination");
                //    }
                //    catch
                //    {
                //        // Ignore the exception which may be generated if the synthesizer is already in the middle of saying something 
                //    } 
                //}
                HideNavBar();
                currentPoint++;
            }
        }

        
    }
}