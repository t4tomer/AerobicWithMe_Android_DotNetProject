using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using AerobicWithMe.ViewModels;

using AerobicWithMe.Views; // Correct namespace for TestPage
using Realms;
using Microsoft.Maui.Controls; // Required for navigation
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using Realms.Sync;


using Microsoft.Maui.Controls; // Required for navigation
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage

using Realms.Sync;

namespace AerobicWithMe.ViewModels

{
    public partial class EditUserRecordViewModel : BaseViewModel, IQueryAttributable
    {



        [ObservableProperty]
        private string inputUserName;// value in the xaml page

        [ObservableProperty]
        private UserRecord initialUserRecord;

        [ObservableProperty]
        private string profileNameNew;


        [ObservableProperty]
        private string mapNameNew;


        [ObservableProperty]
        private string trackTimeNew;

        [ObservableProperty]
        private string uploadDateTimeNew;

        [ObservableProperty]
        private string comment;


        [ObservableProperty]
        private string pageHeader;

        public EditUserRecordViewModel()
        {
            Console.WriteLine($"----> empty constructor,EditMapPinViewModel");
            var singleton = ObjectSingleton.Instance;
            singleton.SetUserRecordType();

            var realm = RealmService.GetMainThreadRealm();


        }






        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {



            if (query.Count > 0 && query["userrecord"] != null) // we're editing an User Record
            {

                InitialUserRecord = query["userrecord"] as UserRecord;
                ProfileNameNew = InitialUserRecord.ProfileName;
                MapNameNew = InitialUserRecord.MapName;
                TrackTimeNew = InitialUserRecord.TrackTime;
                UploadDateTimeNew= InitialUserRecord.UploadDateTime;
                Comment = InitialUserRecord.Comment;

            }
            else // we're creating a new user record
            {

                ProfileNameNew = "";
                MapNameNew = "";
                TrackTimeNew = "";
                UploadDateTimeNew = "";
                Comment = "";
 

                PageHeader = "Create new Record User ";
            }
        }






        public async Task SaveUserRecord(UserRecord newUserRecord) { 
            Console.WriteLine($"SaveUserRecord EditUserRecordViewModel -->");

            string InputProfileName = newUserRecord.ProfileName;
            string InputMapName=newUserRecord.MapName;
            string InputTrackTime = newUserRecord.TrackTime;
            string InputUploadDateTime = newUserRecord.UploadDateTime;
            string InputCommentText = newUserRecord.Comment;

            var singleton = ObjectSingleton.Instance;
            singleton.SetUserRecordType();

            var realm = RealmService.GetMainThreadRealm();

            var userRecordsSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

            if (!userRecordsSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var userRecordQuery = realm.All<UserRecord>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(userRecordQuery, new SubscriptionOptions { Name = "DogSubscription" });
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
                if (InitialUserRecord != null) // editing an item
                {
                    InitialUserRecord.ProfileName = profileNameNew;
                    InitialUserRecord.MapName = mapNameNew;
                    InitialUserRecord.TrackTime = trackTimeNew;
                    InitialUserRecord.UploadDateTime = uploadDateTimeNew;

                }
                else // creating a new user record 
                {
                    realm.Add(new UserRecord()
                    {
                        OwnerId = RealmService.CurrentUser.Id,
                        ProfileName = InputProfileName,
                        MapName = InputMapName,
                        TrackTime = InputTrackTime,
                        UploadDateTime = InputUploadDateTime,
                        Comment = InputCommentText
                    });
                }
            });




            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }












        [RelayCommand]
        public async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

