using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using AerobicWithMe.ViewModels;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage

using Realms.Sync;





namespace AerobicWithMe.ViewModels

{
    public partial class EditMapPinViewModel : BaseViewModel, IQueryAttributable
    {




        [ObservableProperty]
        private MapPin initialMapPin;

        [ObservableProperty]
        private string inputTrackName;// value in the xaml page


        [ObservableProperty]
        private string map_nameNew;


        [ObservableProperty]
        private string label_pinNew;


        [ObservableProperty]
        private string addressNew;


        [ObservableProperty]
        private string latitudeNew;


        [ObservableProperty]
        private string longtiudeNew;



        [ObservableProperty]
        private string pageHeader;

        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        private Maui.GoogleMaps.Map myMap;
        private MapUtility MapHelperObject; // OBEJECT   THAT is used to show track of map
        public EditMapPinViewModel()
        {
            MapHelperObject = new MapUtility(); // Initialize m in the constructor



        }
        public void setMapName(string newMapName)
        {
            this.inputTrackName = newMapName;
        }



        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {



            if (query.Count > 0 && query["mappin"] != null) // we're editing an Item
            {

                InitialMapPin = query["mappin"] as MapPin;
                Map_nameNew = InitialMapPin.Mapname;
                Label_pinNew = InitialMapPin.Labelpin;
                AddressNew = InitialMapPin.Address;
                LatitudeNew = InitialMapPin.Latitude;
                LongtiudeNew = InitialMapPin.Longitude;
                PageHeader = $"Modify Map: {InitialMapPin.Mapname}(PinMap)";
            }
            else // we're creating a new pin map
            {
                Map_nameNew = "";
                Label_pinNew = "";
                AddressNew = "";
                LatitudeNew = "";
                LongtiudeNew = "";

                PageHeader = "Create a New Map";
            }
        }

        //Show the track of pins that are stored in realm db 
        [RelayCommand]
        public async Task addPointsToTrack()
        {

            string trackName = InitialMapPin.Mapname;
            var realm = RealmService.GetMainThreadRealm();

            // Query Realm for all items with a matching Summary.
            var matchingMapPins = realm.All<MapPin>().Where(i => i.Mapname == trackName);

            var mapPinsList = realm.All<MapPin>().ToList(); // Fetch all items into memory

            // Now you can safely use Select
            var summaries = mapPinsList
                .Where(i => i.Mapname == trackName)  // Filter if needed
                .Select(i => new Maui.GoogleMaps.Pin
                {
                    Label = i.Labelpin,
                    Address = i.Address,
                    Position = new Position(Convert.ToDouble(i.Latitude), Convert.ToDouble(i.Longitude))
                })
                .ToList();

            // Loop through the matching items and print their Summary.
            foreach (var pin in summaries)
            {
                Console.WriteLine($"Address of pin (MapHelper class) -->pin label:'{pin.Label}'pin addr: {pin.Address}");
            }


            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.Instance;
            List<Maui.GoogleMaps.Pin> pinList = MapPage.Instance.GetPinList();
            mapPage.setPinsList(summaries);
            mapPage.addPointsToTrack_Clicked();

            if (await mapPage.IsLocationEnabled())
            {
                if (InitialMapPin.IsMine)
                {
                    Console.WriteLine($"-->Track is  mine!!!");
                    mapPage.ShowButtonsOnMap(true); // show buttons 
                    mapPage._canAddPins = true;
                    await Shell.Current.Navigation.PushAsync(mapPage);
                }
                else
                {
                    Console.WriteLine($"-->Track is not mine!!!");
                    mapPage.ShowButtonsOnMap(false); // Remove buttons from the map 
                    mapPage._canAddPins = false;
                    await Shell.Current.Navigation.PushAsync(mapPage);

                }
            }




            if (!matchingMapPins.Any())
            {
                Console.WriteLine($"No pinmaps found with the summary: {trackName}");
            }


        }

        [RelayCommand]
        public async Task PrintList()
        {
            Console.WriteLine("PrintList --EditItemViewModel.");
            if (MapPage.Instance == null)
                Console.WriteLine("MapPage instance is null.");
            else
                Console.WriteLine($"MapPage instance initialized with {MapPage.Instance.GetPinList().Count} pins.");



            List<Maui.GoogleMaps.Pin> pinsList1 = MapPage.Instance.GetPinList();

        }


        //TODO nned to fix this method 
        [RelayCommand]
        public async Task UploadToCloudPins()
        {
            Console.WriteLine($"UploadToCloudPins maptrack name is: {InputTrackName}");

            if (string.IsNullOrEmpty(InputTrackName))
            {
                Console.WriteLine($"---------> empty InputTrackName ");
                await DialogService.ShowAlertAsync("Error", "Can Not Enter Empty Track Name.", "OK");
                return;
            }





            List<Maui.GoogleMaps.Pin> pinsList = MapPage.Instance.GetPinList();

            Track new_Track = new Track(InputTrackName, pinsList);//create new Track object
            await new_Track.UploadToMongoDb();


        }

        /*
                public async Task SavePin(Maui.GoogleMaps.Pin newPin)
                {
                    Console.WriteLine($"SavePin EditMapPin -->'{newPin.Label}': {newPin.Address}");

                    var singleton = TypeFactory.Instance;
                    singleton.SetMapPinType();


                    var realm = RealmService.GetMainThreadRealm();



                    var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

                    if (!mapPinSubscriptionExists)
                    {
                        Console.WriteLine("No existing subscription for MapPin. Adding one now...");

                        // Add the subscription synchronously
                        realm.Subscriptions.Update(() =>
                        {
                            var mapPinQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                            realm.Subscriptions.Add(mapPinQuery, new SubscriptionOptions { Name = "DogSubscription" });
                        });

                        Console.WriteLine("MapPin subscription added. Waiting for synchronization...");

                        // Wait for synchronization
                        await realm.Subscriptions.WaitForSynchronizationAsync();
                        Console.WriteLine("MapPin synchronized successfully.");
                    }
                    else
                    {
                        Console.WriteLine("MapPin subscription already exists.");
                    }






                    await realm.WriteAsync(() =>
                    {
                        if (InitialMapPin != null) // editing an item
                        {
                            InitialMapPin.Mapname = InputTrackName;
                            InitialMapPin.Labelpin = Label_pinNew;
                            InitialMapPin.Address = AddressNew;
                            InitialMapPin.Latitude = LatitudeNew;
                            InitialMapPin.Longitude = LongtiudeNew;

                        }
                        else // creating a new item
                        {
                            realm.Add(new MapPin()
                            {
                                OwnerId = RealmService.CurrentUser.Id,
                                Mapname = InputTrackName,
                                Labelpin = newPin.Label,
                                Address = newPin.Address,
                                Latitude = newPin.Position.Latitude.ToString(),
                                Longitude = newPin.Position.Longitude.ToString()
                            });
                        }
                    });




                    Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
                    await Shell.Current.GoToAsync("..");
                }
                */




        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

