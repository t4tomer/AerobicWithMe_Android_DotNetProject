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
            var mapPage = MapPage.GetInstance;
            List<Maui.GoogleMaps.Pin> pinList = MapPage.GetInstance.GetPinList();
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
            if (MapPage.GetInstance == null)
                Console.WriteLine("MapPage instance is null.");
            else
                Console.WriteLine($"MapPage instance initialized with {MapPage.GetInstance.GetPinList().Count} pins.");



            List<Maui.GoogleMaps.Pin> pinsList1 = MapPage.GetInstance.GetPinList();

        }


        private static bool CheckMapNameAlreadyExists(string inputTrackName)
        {

            //var singleton = ObjectMongoFactory.Instance;
            //singleton.SetTrackMongoType();

            ObjectMongoFactory UploadObjectToMong = new ObjectMongoFactory();
            UploadObjectToMong.CreateTrackMongo();
            


            var realm = RealmService.GetMainThreadRealm();

            // Check if any MongoTrack1 object already has the same map name
            bool exists = realm.All<TrackMongo1>().Any(t => t.TrackName == inputTrackName);

            return exists; // Return false if it exists, true otherwise
        }


  


        //TODO nned to fix this method 
        [RelayCommand]
        public async Task UploadMapPinAndTrackObject()
        {
            Console.WriteLine($"UploadMapPinAndTrackObject() maptrack name is: {InputTrackName}");

            if (string.IsNullOrEmpty(InputTrackName))
            {
                await DialogService.ShowAlertAsync("Error", "Can Not Enter Empty Track Name.", "OK");
                return;
            }
            if (CheckMapNameAlreadyExists(InputTrackName))
            {
                await DialogService.ShowAlertAsync("Error", "TrackName Allready Exists ,enter diffrent name", "OK");
                return;
            }





            List<Maui.GoogleMaps.Pin> pinsList = MapPage.GetInstance.GetPinList();


            Track addNewTrack = new Track(InputTrackName, pinsList);
            var logger = new TrackLogger();

            // Attach observer
            addNewTrack.Attach(logger);
            await addNewTrack.AddTrack();

            // Detach observer when not needed
            addNewTrack.Detach(logger);

        }





        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

