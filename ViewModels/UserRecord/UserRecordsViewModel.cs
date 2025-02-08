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
using System.Globalization;
//using WebKit;



namespace AerobicWithMe.ViewModels
{
    public partial class UserRecordsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string connectionStatusIcon = "wifi_on.png";

        [ObservableProperty]
        private bool isShowAllTasks;

        [ObservableProperty]
        private IQueryable<MapPin> maps;

        [ObservableProperty]
        public string dataExplorerLink = RealmService.DataExplorerLink;

        
        [ObservableProperty]
        private IQueryable<UserRecord> userRecordsList;


        // List of sorting options for the picker
        [ObservableProperty]
        public List<string> _SortOptions = new List<string> {"Profile Name", "Record Time","Upload Date", "Default"};

        // Selected sorting option
        [ObservableProperty]
        public string selectedSortOption= "Default";



        private string _mapTitle = "Test"; // Default value



        private Realm realm;
        private string currentUserId;
        private bool isOnline = true;
        public string _trackName = "Deafult"; // Default value

        private static UserRecordsViewModel _instance;
        private static readonly object _lock = new();
        public ICommand NavigateCommand { get; private set; }

        public UserRecordsViewModel()
        {

            //set singlton to userrecord 
            var singleton = TypeFactory.Instance;
            singleton.SetUserRecordType();

            realm = RealmService.GetMainThreadRealm();
            currentUserId = RealmService.CurrentUser.Id;
            Console.WriteLine($"--> UserRecordsViewModel -empty constructor");

        }




        // used to user record   from the user recorods list 
        [RelayCommand]
        public async Task DeleteUserRecord(UserRecord userRecordFromList)
        {
            string userRecordComment = userRecordFromList.Comment;

            Console.WriteLine($"---------->>> the comment of the user is:{userRecordComment} ");

            //await DialogService.ShowAlertAsync("Error", "You cannot delete user record that is not belonging to you", "OK");

            if (!await CheckUserRecordOwnership(userRecordFromList))
            {
                return;
            }

            await realm.WriteAsync(() =>
            {
                realm.Remove(userRecordFromList);
            });
        
            // Refresh the list after deletion
            OnAppearing();
        }


        // used to user record   from the user recorods list 
        [RelayCommand]
        public async Task ShowRecord(UserRecord userRecordFromList)
        {
            string userRecordComment = userRecordFromList.Comment;

            Console.WriteLine($"---------->>> the comment of the ShowRecord user is:{userRecordComment} ");
            //var userRecordDetailsPage = new UserRecordDetails(); //TODO fix  problem here 
            //await Shell.Current.Navigation.PushAsync(userRecordDetailsPage);


            var currentUserRecordDetails = new UserDetails(); //TODO fix  problem here 
            currentUserRecordDetails.setXAML_Values(userRecordFromList);
            await Shell.Current.Navigation.PushAsync(currentUserRecordDetails);

            //UserRecordDetails



            //userRecordDetailsPage.setUserRecord(userRecordFromList);
            //await Shell.Current.Navigation.PushAsync(userRecordDetailsPage);//1 way 

            OnAppearing();
        }



 





        private async Task<bool> CheckUserRecordOwnership(UserRecord userRecordFromList)
        {
            if (!userRecordFromList.IsMine)
            {
                await DialogService.ShowAlertAsync("Error", "You cannot delete user record that is not belonging to you", "OK");
                return false;
            }

            return true;
        }





        public string UserRecordsTiltle //XAML lavbel of the Title 
        {
            get => $" Map :{_trackName} user records";
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged(nameof(UserRecordsTiltle)); // Notify the UI about the change
                }
            }
        }


        public void setTrackName(string newTitle)
        {
            _trackName = newTitle; // Update the internal mapTitle field
            Console.WriteLine($"-->track name (UserRecordsViewModel): {_trackName}");
        }

        public string getTrackName()
        {
            return _trackName; 
        }



        [RelayCommand]

        public async void OnAppearing()
        {
            Console.WriteLine($"----> SelectedSortOption value :{SelectedSortOption}");
            //OnSelectedSortOptionChanged(selectedSortOption);//Show List by sorting the records
            // Set the singleton object to UserRecord type
            var singleton = TypeFactory.Instance;
            singleton.SetUserRecordType();
            realm = RealmService.GetMainThreadRealm();

            // Check if the subscription for UserRecord type exists,if not create new subscription
            var userRecordSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "UserRecordSubscription");

            if (!userRecordSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for UserRecord. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {

                    // Query to include all UserRecords
                    var allUserRecordsQuery = realm.All<UserRecord>();

                    realm.Subscriptions.Add(allUserRecordsQuery, new SubscriptionOptions { Name = "UserRecordSubscription" });
               
                });

                Console.WriteLine("UserRecord subscription added. Waiting for synchronization...");

                // Wait for synchronization
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("UserRecord subscription already exists.");
            }

            //TODO show the user's record by sorting 
            SortUserRecords();

            //show all the useres with the same MapName field
            if (_trackName != "Deafult")
            {
                //list with  all user records with same MapName
                UserRecordsList = getUserRecrodsWithTheSameTrackName();

            }
            else
                UserRecordsList = realm.All<UserRecord>();


            // Show only the current user's(the user that is currently logged to the app) UserRecords
            if (!IsShowAllTasks)
            {
                //list with all the user records that is currently logged to the app
                UserRecordsList = getUserRecrodsOfCurrentUser();

            }



            //-------> used for testing !!!

            /*
            Console.WriteLine($"----> Displaying all UserRecords with MapName: {_trackName}");
            foreach (var userRecord in UserRecordsList)
            {
                Console.WriteLine($"Id: {userRecord.Id}, ProfileName: {userRecord.ProfileName}, OwnerId: {userRecord.OwnerId}, MapName: {userRecord.MapName}");
            }
            */

            var currentSubscriptionType = RealmService.GetCurrentSubscriptionType(realm);
            IsShowAllTasks = currentSubscriptionType == SubscriptionType.All;// TODO cheack this code line 

        }



        //get the user records of the user that is currenntly logged to the app
        private IQueryable<UserRecord> getUserRecrodsOfCurrentUser()
        {
            IQueryable<UserRecord>  UserRecordsListOfCurrentUser = UserRecordsList
            .Where(record => record.OwnerId == currentUserId)
            .AsQueryable();
            return UserRecordsListOfCurrentUser;
        }

        // get the user records with the same track name
        private IQueryable<UserRecord> getUserRecrodsWithTheSameTrackName()
        {
            //all user records with same MapName
            IQueryable<UserRecord> UserRecordsListSameTrackName = realm.All<UserRecord>()
            .Where(record => record.MapName == _trackName)
            .AsQueryable();
            return UserRecordsListSameTrackName;
        }






        [RelayCommand]
        public async Task Logout()
        {
            IsBusy = true;
            await RealmService.LogoutAsync();
            IsBusy = false;

            await Shell.Current.GoToAsync($"//login");
        }



 








        // used to transfer the user to the map page 
        [RelayCommand]
        public async Task ToMapPage()
        {
   


            // Navigate to the singleton instance of MapPage
            var mapPage = MapPage.Instance;
            mapPage.SetTitle("Create new map");
            List<Maui.GoogleMaps.Pin> pinList = MapPage.Instance.GetPinList();
            if (await mapPage.IsLocationEnabled())
            {

                mapPage.ClearMap();
                mapPage.ShowButtonsOnMap(true);//show buttons on map
                mapPage._canAddPins = true;// user can add pins on map
                await Shell.Current.Navigation.PushAsync(mapPage);
            }
        }









        // used to delete map from the maps view 
        [RelayCommand]
        public async Task DeleteMap(MapPin pin)
        {


            if (!await CheckItemOwnership(pin))
            {
                return;
            }

            // Query all maps with the same mapname
            var mapToDelete = realm.All<MapPin>()
                .Where(i => i.Mapname == pin.Mapname)
                .ToList();


            foreach (var pinsInMap in mapToDelete)
            {
                await DeleteSinglePin(pinsInMap);
            }
            // Refresh the list after deletion
            OnAppearing();
        }

        //delete single pin from map 
        [RelayCommand]
        public async Task DeleteSinglePin(MapPin pin)
        {

            //Console.WriteLine($"--->(DeleteItem) item summery:{item.Summary} ");

            if (!await CheckItemOwnership(pin))
            {
                return;
            }

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
            var timerPage = TimerPage.Instance;
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

        [RelayCommand]
        public async Task UrlTap(string url)
        {
            await Launcher.OpenAsync(DataExplorerLink);
        }

        private async Task<bool> CheckItemOwnership(MapPin map)
        {
            //if (!item.IsMine)
            //{
            //    await DialogService.ShowAlertAsync("Error", "You cannot modify items not belonging to you", "OK");
            //    return false;
            //}

            return true;
        }


        [RelayCommand]
        public async Task GoToMapsList()
        {
            //go to the maps list
            await Shell.Current.GoToAsync($"//maps");





        }



        //TODO option tag  DropDown picker in the user records page 

        // when the picker is chosen : Date,
        partial void OnSelectedSortOptionChanged(string value)
        {
            // no sorting is required 
            if (value == "Default" || value== "Upload Date")
            {

                Console.WriteLine($"=======>>>  sorting by Upload Date or Default!!!!");

                UserRecordsList = getUserRecrodsWithTheSameTrackName();

                foreach (var userRecord in UserRecordsList)
                {
                    Console.WriteLine($"ProfileName(Default or Uploaddate): {userRecord.ProfileName}, Upload Date: {userRecord.UploadDateTime}");
                }


                return;

            }
            SortUserRecords();
        }

        // Method to handle sorting logic when the user selects an option
        [RelayCommand]
        public void SortUserRecords()
        {
            var singleton = TypeFactory.Instance;
            singleton.SetUserRecordType();
            realm = RealmService.GetMainThreadRealm();


            if (SelectedSortOption == "Record Time")
            {
                Console.WriteLine($"=======>>>  sorting by Record Time!!!!");



                var userRecordsList = getUserRecrodsWithTheSameTrackName().ToList();


                // Sort the user records with same map name by track time
                var sortedUserRecords = userRecordsList
                    .Where(record => TimeSpan.TryParse(record.TrackTime, out _)) // Ensure valid times
                    .OrderBy(record => TimeSpan.Parse(record.TrackTime))
                    .ToList();

                UserRecordsList = sortedUserRecords.AsQueryable();

                //used for testing 
                foreach (var userRecord in sortedUserRecords)
                {
                    Console.WriteLine($"ProfileName: {userRecord.ProfileName}, Record Time: {userRecord.TrackTime}");
                }



            }
            else if (SelectedSortOption == "Profile Name")
            {
                Console.WriteLine($"=======>>> sort by Profile name !!!!");

                var sortedUserRecords = UserRecordsList.OrderBy(record => record.ProfileName);

                foreach (var userRecord in sortedUserRecords)
                {
                    Console.WriteLine($"ProfileName: {userRecord.ProfileName}, Upload Date: {userRecord.UploadDateTime}");
                }

                UserRecordsList = sortedUserRecords.AsQueryable();


            }
        }





        async partial void OnIsShowAllTasksChanged(bool value)
        {
            if(value)
                await DialogService.ShowAlertAsync("Switch", "Showing all Useres Records", "OK");
            //else
            //    await DialogService.ShowAlertAsync("Switch", "Showing only my user records", "OK");

            await RealmService.SetSubscription(realm, value ? SubscriptionType.All : SubscriptionType.Mine);

            if (!isOnline)
            {
                await DialogService.ShowToast("Switching subscriptions does not affect Realm data when the sync is offline.");
            }
            Refresh();//refresh the items list 
        }
    }
}
