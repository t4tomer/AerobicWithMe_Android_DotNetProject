using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Maui.GoogleMaps;
using AerobicWithMe.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Input;
using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Services;
using Realms;
using AerobicWithMe.Views; // Correct namespace for TestPage
using Microsoft.Maui.Controls; // Required for navigation
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
//using static Xamarin.Google.Crypto.Tink.Shaded.Protobuf.Internal;


namespace AerobicWithMe.Views
{
    public partial class MapPage : ContentPage, INotifyPropertyChanged
    {
        private static MapPage _instance; // Singleton instance
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        private MapHelper MapHelperObject; // Declare m as a class-level variable
        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        public bool _canAddPins = true; // Controls if pins can be added
        private string _mapTitle = ""; // Default value
        int strokeColorPolyline = 0;

        public ICommand NavigateCommand { get; private set; }
        int randomNumberTest = 1;

        // Private constructor to prevent direct instantiation
        private MapPage()
        {
            InitializeComponent();
            BindingContext = this; // Set the binding context for data binding

        }

        // Public static property to get the singleton instance
        public static MapPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapPage();
                }
                return _instance;
            }
        }

        public string MapTitle
        {
            get => $"Track :{_mapTitle}";
            set
            {
                if (_mapTitle != value)
                {
                    _mapTitle = value;
                    OnPropertyChanged(nameof(MapTitle)); // Notify the UI about the change
                }
            }
        }

        public void SetTitle(string newTitle)
        {
            _mapTitle = newTitle; // Update the internal mapTitle field
            Console.WriteLine($"Map title updated to: {newTitle}");
        }


        public void setPinsList(List<Maui.GoogleMaps.Pin> newpinstList)
        {
            this.pinsList = newpinstList;
        }

        public async Task<bool> IsLocationEnabled()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                // If we get a valid location, services are likely enabled.
                if (location != null)
                {
                    Console.WriteLine($"Location: Lat {location.Latitude}, Lon {location.Longitude}");
                    return true;
                }
                else
                {
                    Console.WriteLine("No location found. Location services may be off.");
                    await DialogService.ShowAlertAsync("Error", "No location found. Location services may be off", "OK");

                    return false;
                }
            }
            catch (FeatureNotEnabledException)
            {
                Console.WriteLine("Location services are disabled.");
                await DialogService.ShowAlertAsync("Error", "Turnon location on your device", "OK");

                return false;
            }
            catch (PermissionException)
            {
                Console.WriteLine("Location permission not granted.");
                await DialogService.ShowAlertAsync("Error", "Turnon location on your device", "OK");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                await DialogService.ShowAlertAsync("Error", "Unexpected error", "OK");

                return false;
            }
        }

        // Public method to get the list of pins from the map
        public List<Maui.GoogleMaps.Pin> GetPinList()
        {
            return myMap.Pins.ToList();
        }

        private List<Maui.GoogleMaps.Pin> GetPins(object sender, EventArgs e)
        {
            return myMap.Pins.ToList();
        }

        // Method to print the number of points on the map
        private void numberOfPoints(object sender, MapClickedEventArgs e)
        {
            int pointCount = myMap.Pins.Count;
            Console.WriteLine($"----> Number of points on the map!!!: {pointCount}");
        }
        // create random number 
        //public int GenerateRandomNumber()
        //{
        //    Random random = new Random();
        //    int randomNumber = random.Next(1, 1000); // The upper bound is exclusive, so use 1000 to get numbers from 1 to 999
        //    return randomNumber;
        //}
        // method that is used to zoom to my location
        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    CenterMap(location.Latitude, location.Longitude);

                }

                return location;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }


        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
        // zoom the google map to the cordinates- of x and y 
        private void CenterMap(double x, double y)
        {
            //var position = new Position(31.268333463883636, 34.80691033370654);
            var position = new Position(x, y);
            var mapSpan = Maui.GoogleMaps.MapSpan.FromCenterAndRadius(position, Maui.GoogleMaps.Distance.FromMeters(1)); // Adjust the radius as needed
            myMap.MoveToRegion(mapSpan);
        }






        [RelayCommand]
        public async Task GoToTimerPageButton_Pressed()//transfer to timer page
        {
            // Navigate to the singleton instance of TimerPage
            var timerPage = TimerPage.Instance;
            timerPage.setTitle(_mapTitle);
            await Shell.Current.Navigation.PushAsync(timerPage);
        }


        [RelayCommand]
        public async Task ZoomToMyLocationButton_Pressed()
        {
            GetCurrentLocation();

        }


        [RelayCommand]
        public async Task GoToUserRecordsListButton_Pressed()
        {

            UserRecordsViewModel userRecordsVM_Page = new UserRecordsViewModel();

            userRecordsVM_Page.setTrackName(_mapTitle);

            // Create a new instance of the UserRecordsPage and bind it to the ViewModel
            var userRecordsPage = new UserRecordsPage
            {
                BindingContext = userRecordsVM_Page
            };

            // Navigate to the page
            await Navigation.PushAsync(userRecordsPage);


        }


        [RelayCommand]

        public async Task DeletLastPointButton_Pressed()
        {
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();

            Console.WriteLine($" \t\t-->Number of pins:(MapPage)" + pinsList.Count);

            MapHelperObject.deleteLastPoint(pinsList);
        }


        [RelayCommand]

        public async Task AddToCloudButton_Pressed()
        {

            Console.WriteLine($"-->AddToCloud_Clicked ");

            //AddMapToDbPage
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount = pinsList.Count;
            if (await EnoughPins(pinCount))
            {
                MapHelperObject = new MapHelper(pinsList, myMap);
                //MapHelperObject.PrintPinAddresses();// used for testing
                var AddToCloud = new AddMapToDbPage(pinsList, myMap);
                await Navigation.PushAsync(AddToCloud);
            }
        }


        // calculate the distance between all the points on the map
        [RelayCommand]

        public async Task CalcDistanceButton_Pressed()
        {


            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount = pinsList.Count;
            if (await EnoughPins(pinCount))
            {
                //MapHelperObject = new MapHelper(pinsList,myMap); // Initialize m in the constructor

                MapHelperObject.setPinsList(pinsList);
                double totalDistance = MapHelperObject.calculateTotalDistance();


                // Create an instance of TestPage and pass the total distance
                var currentDistancePage = new DistancePage(totalDistance, pinsList, MapHelperObject);

                // Navigate to the TestPage
                await Navigation.PushAsync(currentDistancePage);
            }


        }


        // remove all the points&polylines from the map 
        [RelayCommand]

        public async Task ResetMapButton_Pressed() {

            ClearMap();

        }


        // method that is used to transfer the user to the edit point page
        [RelayCommand]

        public async Task EditPointButton_Pressed()
        {
            Console.WriteLine($"----> Edit Point clicked ");

            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();
            int pinCount = pinsList.Count;
            //Console.WriteLine($"number of pins in the list(Add_Point_Clicked): -->'{pinCount}': {pinCount}");

            var EditPinAddrPage = new EditPinAddr(pinsList, myMap);
            //EditPinAddrPage.setMapName(MapTitle);
            //! Pass the pinsList directly when navigating to the triggerPage
            await Navigation.PushAsync(EditPinAddrPage);

        }





        // add new point on the map 
        private void addPointOnMap(object sender, MapClickedEventArgs e)
        {

            if (!_canAddPins) return; // Stop if adding pins is disabled


            double latitude = e.Point.Latitude;
            double longitude = e.Point.Longitude;

            // Print the coordinates to the console







            int pinCount = myMap.Pins.Count + 1;

            var pin = new Maui.GoogleMaps.Pin
            {
                Label = pinCount.ToString(),
                Address = "Adresss" + pinCount.ToString(),
                Position = e.Point,
                Type = PinType.Place
            };

            strokeColorPolyline++;


            myMap.Pins.Add(pin);
            List<Maui.GoogleMaps.Pin> pinsList = myMap.Pins.ToList();

            MapHelperObject = new MapHelper(pinsList, myMap); 
            MapHelperObject.drawLineBetweenAllPins(strokeColorPolyline);

        }

        // Print all addresses of the points 
        private void PrintPinAddresses(object sender, MapClickedEventArgs e)
        {
            var pins = myMap.Pins.ToList();
            foreach (var pin in pins)
            {
                Console.WriteLine($"Address of pin22 -->'{pin.Label}': {pin.Address}");
            }
        }









        //added the ShowButtonsOnMap method 

        public void ShowButtonsOnMap(bool cond)
        {
            EditPointButton.IsVisible = cond;
            ClearMapButton.IsVisible = cond;
            AddToCloudButton.IsVisible = cond;
            DeleteLastPointButton.IsVisible = cond;
            ZoomButton.IsVisible = true;
            DistanceButton.IsVisible = true;

        }

        public void ShowStartExerciseButton(bool cond)
        {
            Console.WriteLine($"----> ShowStartExerciseButton condtion:{cond}");

            StartExcericeButton.IsVisible = cond;
        }

        public void ShowUsersRecordsButton(bool cond)
        {
            Console.WriteLine($"----> ShowUsersRecordsButton condtion:{cond}");

            UsersRecordsButton.IsVisible = cond;
        }



        public void ShowTrack_Clicked()//show the pins and the lines of the track 
        {

            ClearMap();//clear the map from previus pins 

            MapHelperObject = new MapHelper(pinsList, myMap);
            MapHelperObject.showTrackOnMap();//show the pins on the map .
            MapHelperObject.drawLineBetweenAllPins(strokeColorPolyline);//draw line between all the pins of the map .

        }




        // cheack if there are enought opins on the track in order to upload it .
        private async Task<bool> EnoughPins(int num)
        {
            if (num == 0)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add at least 2 points).", "OK");
                return false;
            }
            else if (num == 1)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add 1 more point).", "OK");
                return false;
            }
            return true;

        }



        public void ClearMap()
        {
            Console.WriteLine($"----> Remove all Points  ");
            // Clear all pins from the map
            myMap.Pins.Clear();
            // Clear all polylines from the map
            myMap.Polylines.Clear();

            Console.WriteLine("----> All pins and polylines have been cleared.");
        }





        private async void toPage(string pageName)
        {
            Debug.WriteLine("toPage function!!!!"); // Write a debug message indicating the method was called

            // Construct the full type name including namespace
            string fullTypeName = $"MauiGoogleMapsSample.{pageName}";

            // Get the Type object based on the full type name
            Type pageType = Type.GetType(fullTypeName);

            if (pageType != null)
            {
                // Check if NavigateCommand can execute with this Type
                if (NavigateCommand.CanExecute(pageType))
                {
                    NavigateCommand.Execute(pageType);
                }
            }
            else
            {
                Debug.WriteLine($"Page type '{pageName}' not found.");
            }
        }

        // Method to calculate distance between two pins
        private double GetDistance(Maui.GoogleMaps.Pin p1, Maui.GoogleMaps.Pin p2)
        {
            // Access the Position property of each Pin
            var pos1 = p1.Position;
            var pos2 = p2.Position;

            // Convert Position to Location
            Location loc1 = new Location(pos1.Latitude, pos1.Longitude);
            Location loc2 = new Location(pos2.Latitude, pos2.Longitude);

            // Calculate the distance using Location.CalculateDistance
            double distance = Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
            Console.WriteLine($"----> Distance between p1 and p2: {distance} km");

            return distance;
        }



        private Maui.GoogleMaps.Pin getPoint(double x, double y, string inputLabel, string inputAddress)
        {
            var position = new Location(x, y);
            var pin = new Maui.GoogleMaps.Pin
            {
                Label = inputLabel,
                Address = inputAddress,
                Type = PinType.Place,
                Position = new Position(position.Latitude, position.Longitude)
            };

            myMap.Pins.Add(pin);
            return pin;
        }
    }
}
