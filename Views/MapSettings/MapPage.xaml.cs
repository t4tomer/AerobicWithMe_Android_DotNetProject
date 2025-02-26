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
using AerobicWithMe.Commands;
//using static Xamarin.Google.Crypto.Tink.Shaded.Protobuf.Internal;


namespace AerobicWithMe.Views
{
    public partial class MapPage : ContentPage, INotifyPropertyChanged
    {
        private static MapPage _instance; // Singleton instance
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        private MapUtility MapHelperObject; // Declare m as a class-level variable
        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        public bool _canAddPins = true; // Controls if pins can be added
        private string _mapTitle = ""; // Default value
        int strokeColorPolyline = 0;
        private readonly ButtonInvoker _buttonInvoker = new ButtonInvoker();//part of the Command Design pattern 

        //private ButtonInvoker _buttonInvoker;


        public ICommand NavigateCommand { get; private set; }
        int randomNumberTest = 1;

        // Private constructor to prevent direct instantiation
        private MapPage()
        {
            InitializeComponent();
            BindingContext = this; // Set the binding context for data binding

        }

        // Public static property to get the singleton instance
        public static MapPage GetInstance
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

        public string MapTitle //show the map name on the xaml page
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



        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }






        [RelayCommand]
        public async Task GoToTimerPageButton_Pressed()//transfer to timer page
        {

            // use the Command Design Pattern
            NavigateToTimerPageCommand command = new NavigateToTimerPageCommand(_mapTitle);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();


        }


        [RelayCommand]
        public async Task ZoomToMyLocationButton_Pressed()
        {
            //Command Design Pattern
            ZoomToMyLocationCommand command = new ZoomToMyLocationCommand(myMap);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();




        }


        [RelayCommand]
        public async Task GoToUserRecordsListButton_Pressed()
        {

            //Command Design Pattern

            NavigateToRecordsListPageCommand command = new NavigateToRecordsListPageCommand(_mapTitle);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();

        }


        [RelayCommand]

        public async Task DeletLastPointButton_Pressed()
        {

            //Command Design Pattern
            DeleteLastPointAddedCommand command = new DeleteLastPointAddedCommand(myMap);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();
        }


        [RelayCommand]

        public async Task AddToCloudButton_Pressed()
        {
            //Command Design Pattern
            NavigateToUploadTrackPageCommand command = new NavigateToUploadTrackPageCommand(myMap);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();
        }


        // calculate the distance between all the points on the map
        [RelayCommand]

        public async Task CalcDistanceButton_Pressed()
        {




            NavigateToDistancePageCommand command = new NavigateToDistancePageCommand(myMap);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();


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


            ////Command Design Pattern
            NavigateToEditPointCommand command = new NavigateToEditPointCommand(myMap);
            _buttonInvoker.SetCommand(command);
            await _buttonInvoker.PressButton();


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

            MapHelperObject = new MapUtility(pinsList, myMap); 
            MapHelperObject.drawLineBetweenAllPins(strokeColorPolyline);

        }

        // Print all addresses of the points 
        /* //used for testing 
        private void PrintPinAddresses(object sender, MapClickedEventArgs e)
        {
            var pins = myMap.Pins.ToList();
            foreach (var pin in pins)
            {
                Console.WriteLine($"Address of pin22 -->'{pin.Label}': {pin.Address}");
            }
        }
        */









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



        public void addPointsToTrack_Clicked()//show the pins and the lines of the track 
        {

            ClearMap();//clear the map from previus pins 

            MapHelperObject = new MapUtility(pinsList, myMap);
            MapHelperObject.addPointsToTrackOnMap();//show the pins on the map .
            MapHelperObject.drawLineBetweenAllPins(strokeColorPolyline);//draw line between all the pins of the map .

        }






        public void ClearMap()
        {
            myMap.Pins.Clear();
            myMap.Polylines.Clear();

        }





   




    }
}
