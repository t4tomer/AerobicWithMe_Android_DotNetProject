using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using Realms;
using AerobicWithMe.Views; // Correct namespace for TestPage
using Microsoft.Maui.Controls; // Required for navigation
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Position = Maui.GoogleMaps.Position;
using Realms.Sync;



namespace AerobicWithMe.ViewModels
{
    public partial class MapsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<MapPin> maps;



        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;










        [ObservableProperty]
        private string pageHeader;

        //-->End--->used for testing -upload user record object to mongodb 









        public MapsViewModel()
        {
            //set singlton to mappin 
            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetMapPinType();

            SetObjectToUpload UploadObjectToMong = new SetObjectToUpload();
            UploadObjectToMong.CreateMapPin();

            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
        }

        public async void deleteExistingMapPinFromCloude(Maui.GoogleMaps.Pin newPin, string mapNameToDelete)
        {



            var mapPinToDelete = new MapPin
            {
                OwnerId = RealmService.CurrentUser.Id, // Assuming `RealmService` is initialized
                Mapname = mapNameToDelete,
                Labelpin = newPin.Label,
                Address = newPin.Address,
                Latitude = newPin.Position.Latitude.ToString(), // Convert latitude to string
                Longitude = newPin.Position.Longitude.ToString() // Convert longitude to string
            };

            Console.WriteLine($"----> deleteExistingMapPinFromCloude ");

            await DeleteSinglePin(mapPinToDelete);


        }




        [RelayCommand]
        public void OnAppearing()
        {

            // used to reset the timer when switching tracks 
            var timer = TimerPage.GetInstance;
            timer.ResetTimer();

            //set the singlton object to mappin type 

            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetMapPinType();
            SetObjectToUpload uploadObjectToMongo = new SetObjectToUpload();
            uploadObjectToMongo.CreateMapPin();

            realm = RealmService.GetMainThreadRealm();

            // Check if the subscription for MapPin  type exists
            var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "MapPinSubscription");

            if (!mapPinSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var mapPinQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(mapPinQuery, new SubscriptionOptions { Name = "MapPinSubscription" });
                });

                Console.WriteLine("MapPin subscription added. Waiting for synchronization...");

                // Wait for synchronization
                realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("MapPin subscription already exists.");
            }



            // Retrieve all items from Realm and convert them to a list.
            var mapNamesList = realm.All<MapPin>().ToList();

            // Group the MapPin object by the same Mapname and select the first item from each group.
            var distinctMapNames = mapNamesList
                .GroupBy(map => map.Mapname)
                .Select(group => group.First())
                .OrderBy(map => map.Id)
                .ToList();

            // Assign the filtered list back to Maps,showen in XAML page.
            Maps = distinctMapNames.AsQueryable();

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);



            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;
        }


    

        [RelayCommand]
        public async Task GoToUserRecordsList()
        {




            var userRecordsPage = UserRecordsPage.Instance;
            await Shell.Current.Navigation.PushAsync(userRecordsPage);



        }




        [RelayCommand]
        public async Task Logout()
        {
            IsBusy = true;
            await RealmService.LogoutAsync();
            IsBusy = false;

            await Shell.Current.GoToAsync($"//login");
        }


        private static List<Maui.GoogleMaps.Pin> getPinsListByName(string trackName)
        {
            var realm = RealmService.GetMainThreadRealm();

            // Query Realm for all items with a matching Summary.
            var matchingMapPins = realm.All<MapPin>().Where(i => i.Mapname == trackName);

            var mapPinsList = realm.All<MapPin>().ToList(); // Fetch all items into memory

            // Now you can safely use Select
            var pinTypeList = mapPinsList
                .Where(i => i.Mapname == trackName)  // Filter if needed
                .Select(i => new Maui.GoogleMaps.Pin
                {
                    Label = i.Labelpin,
                    Address = i.Address,
                    Position = new Position(Convert.ToDouble(i.Latitude), Convert.ToDouble(i.Longitude))
                })
                .ToList();

            // Loop through the matching items and print their Summary.
            foreach (var pin in pinTypeList)
            {
                Console.WriteLine($"Address of pin (MapHelper class) -->pin label:'{pin.Label}'pin addr: {pin.Address}");
            }

            return pinTypeList;
        }




        //method that is used to edit map
        [RelayCommand]
        public async Task ChooseMapFromList(MapPin map)
        {
            string mapName = map.Mapname;
            Console.WriteLine($"(EditMap)MapsViewModel,mapname:{map.Mapname} ");

            //convert MapPin object with the same mapname to list with the same name but with type of Maui.GoogleMaps.Pin
            List<Maui.GoogleMaps.Pin> pinListOfSameMapName = getPinsListByName(mapName);

            var mapPage = MapPage.GetInstance;
            mapPage.setPinsList(pinListOfSameMapName);
            mapPage.addPointsToTrack_Clicked();
            mapPage.SetTitle(mapName);
            if (await mapPage.IsLocationEnabled())
            {
                if (map.IsMine)
                {
                    Console.WriteLine($"-->Track is  mine!!!");
                    mapPage.ShowStartExerciseButton(true);//show the start exerice button on map
                    mapPage.ShowUsersRecordsButton(true);
                    mapPage.ShowButtonsOnMap(true); // show buttons 
                    mapPage._canAddPins = true;
                    await Shell.Current.Navigation.PushAsync(mapPage);//1 way 
                    //await Shell.Current.GoToAsync($"chooseMapFromList");//2 way

                }
                else
                {
                    Console.WriteLine($"-->Track is not mine!!!");
                    mapPage.ShowStartExerciseButton(true);//show the start exerice button on map
                    mapPage.ShowUsersRecordsButton(true);
                    mapPage.ShowButtonsOnMap(false); // Remove buttons from the map 
                    mapPage._canAddPins = false;
                    await Shell.Current.Navigation.PushAsync(mapPage);//1 way 


}
                }



            }


        







        // used to transfer the user to the map page 
        [RelayCommand]
        public async Task CreateNewTrack()
        {



            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.GetInstance;
            mapPage.SetTitle("Create new map");
            List<Maui.GoogleMaps.Pin> pinList = MapPage.GetInstance.GetPinList();
            if (await mapPage.IsLocationEnabled())
            {
                mapPage.ShowStartExerciseButton(false);
                mapPage.ShowUsersRecordsButton(false);
                mapPage.ClearMap();
                mapPage.ShowButtonsOnMap(true);//show buttons on map
                mapPage._canAddPins = true;// user can add pins on map
                await Shell.Current.Navigation.PushAsync(mapPage);
            }
        }









        // used to delete map from the maps view 
        [RelayCommand]
        public async Task DeleteMap(MapPin pinOfChosenMap)
        {
            string trackNameToDelete = pinOfChosenMap.Mapname;


            if (!await CheckMapOwnership(pinOfChosenMap))
            {
                return;
            }

            if (!await WarningDeletingMyTrack(pinOfChosenMap))
            {
                return;
            }


            //observor desighn pattern 
            List<Maui.GoogleMaps.Pin> pinsList = MapPage.GetInstance.GetPinList();

            Track removeExistingTrack = new Track(trackNameToDelete, pinsList);
            var logger = new TrackLogger();

            // Attach observer
            removeExistingTrack.Attach(logger);
            await removeExistingTrack.RemoveTrack(pinOfChosenMap);


            // Detach observer when not needed
            removeExistingTrack.Detach(logger);

            await DeleteUsersOfTrack(trackNameToDelete);


            // Refresh the list after deleting the chosen track
            OnAppearing();



        }

        public async Task DeleteUsersOfTrack(string trackName)
        {
            UserRecordsViewModel deleteUseres = new UserRecordsViewModel();

            //set singlton to UserRecord 
            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetUserRecordType();

            SetObjectToUpload uploadObjectToMongo = new SetObjectToUpload();
            uploadObjectToMongo.CreateUserRecord();

            realm = RealmService.GetMainThreadRealm();

            // list of user records with the same MapName 
            var listOfUsersRecordsOfTrack = realm.All<UserRecord>()
                .Where(user => user.MapName == trackName)
                .ToList();

            if (listOfUsersRecordsOfTrack.Count() == 0)
                return;

            foreach (var userToDelete in listOfUsersRecordsOfTrack)
            {
                await deleteUseres.DeleteUserRecord(userToDelete);
            }
        }

        //delete single pin from map 
        [RelayCommand]
        public async Task DeleteSinglePin(MapPin pin)
        {

            await realm.WriteAsync(() =>
            {
                realm.Remove(pin);
            });

        }


        [RelayCommand]
        public void Refresh()
        {
            Console.WriteLine($"---> refreshed page ");
            OnAppearing();

        }




        [RelayCommand]
        public async Task ToTimerPage()//transfer to timer page
        {
            // Navigate to the singleton instance of MapPage
            var timerPage = TimerPage.GetInstance;
            await Shell.Current.Navigation.PushAsync(timerPage);
        }






        [RelayCommand]
        public void ChangeConnectionStatus()
        {
            isOnline = !isOnline;

            if (isOnline)
            {
                realm.SyncSession.Start();
            }
            else
            {
                realm.SyncSession.Stop();
            }

            ConnectionStatusIcon = isOnline ? "wifi_on.png" : "wifi_off.png";
        }



        private async Task<bool> WarningDeletingMyTrack(MapPin chosenPinOfMap)
        {
            // Display a dialog with "OK" and "Cancel" and capture the user's response
            bool userConfirmed = await Application.Current.MainPage.DisplayAlert(
                "Warning",
                "Deleting track will delete the user's records of this map",
                "OK",
                "Cancel"
            );

            // Return true if the user pressed "OK", false otherwise
            return userConfirmed;
        }




        private async Task<bool> CheckMapOwnership(MapPin chosenPinOfMap)
        {
            if (!chosenPinOfMap.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot delete tracks not belonging to you", "OK");
                return false;
            }



            return true;
        }

        async partial void OnIsShowAllTasksChanged(bool value)
        {
            if (value)
                await DialogService.ShowAlertAsync("Switch", "Displaying All Usere's Maps", "OK");

            await RealmService.SetSubscription(realm, value ? SubscriptionType.All : SubscriptionType.Mine);

            if (!isOnline)
            {
                await DialogService.ShowToast("Switching subscriptions does not affect Realm data when the sync is offline.");
            }
            Refresh();//refresh the items list 
        }
    }
}
