using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Services;
using AerobicWithMe.ViewModels;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using System.Numerics;

namespace AerobicWithMe.Models
{
    // Observer Interface (Observer)
    public interface ITrackObserver
    {
        void OnTrackUpdated(Track track);
    }

    // Subject Interface (Subject)
    public interface ITrackSubject
    {
        void Attach(ITrackObserver observer);
        void Detach(ITrackObserver observer);
        void Notify();
    }

    // Concrete Subject (Concrete implementation of Subject)

    public partial class Track : ITrackSubject
    {
        private List<ITrackObserver> _observers = new List<ITrackObserver>();

        private List<Maui.GoogleMaps.Pin> _pinsList;
        private string _inputTrackName;



        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }


        [MapTo("TrackName")]//The variable that appears on the MONGO DB website
        [Required]
        public string TrackName { get; set; }

        [MapTo("DateOfCreationTheTrack")]//The variable that appears on the MONGO DB website
        [Required]
        public string DateOfCreation { get; set; }

        [MapTo("NumberOFPinsInTheTrack")]//The variable that appears on the MONGO DB website
        public int NumberOfPins { get; set; }


        [MapTo("DistanceOfTrackKm")]//The variable that appears on the MONGO DB website
        public double DistanceOfTrack { get; set; }





        public List<Maui.GoogleMaps.Pin> PinsList
        {
            get => _pinsList;
            set
            {
                _pinsList = value;
                Notify(); // Notify observers when the list changes
            }
        }

        public Track(string newInputTrackName, List<Maui.GoogleMaps.Pin> newPinsList)
        {
            _inputTrackName = newInputTrackName;
            _pinsList = newPinsList;
        }

        public void Attach(ITrackObserver observer)
        {
            _observers.Add(observer);
            Console.WriteLine("Observer attached.");
        }

        public void Detach(ITrackObserver observer)
        {
            _observers.Remove(observer);
            Console.WriteLine("Observer detached.");
        }

        public void Notify()
        {
            Console.WriteLine("Notifying observers...");
            foreach (var observer in _observers)
            {
                observer.OnTrackUpdated(this);
            }
            //return the tracks page
            Shell.Current.GoToAsync("..");

        }



        public async Task AddTrack()
        {
            int pinNumber = 0;
            foreach (var pin in _pinsList)
            {
                pinNumber++;
                Console.WriteLine($"----->pinNumber -->'{pinNumber}");

                Console.WriteLine($"Uploading Pin -->'{pin.Label}': {pin.Address}");
                await SavePin(pin, pinNumber);
            }

            await SaveTrack(firstPinOfListAddedLast());
            Notify(); // Notify observers after upload
        }

        public async Task RemoveTrack(MapPin pinOfChoseMap)
        {

            string trackNameToDelete = pinOfChoseMap.Mapname;

            var singleton = TypeFactory.Instance;
            singleton.SetMapPinType();
            var realm = RealmService.GetMainThreadRealm();

            var mapToDelete = realm.All<MapPin>()
                .Where(track => track.Mapname == trackNameToDelete)
                .ToList();

            foreach (var pin in mapToDelete)
            {
                await DeleteSinglePin(pin);
            }

            Notify(); // Notify observers after deletion
        }

        private async Task DeleteSinglePin(MapPin pin)
        {
            var singleton = TypeFactory.Instance;
            singleton.SetMapPinType();

            var realm = RealmService.GetMainThreadRealm();
            await realm.WriteAsync(() => realm.Remove(pin));
        }

        public async Task SaveDog()
        {
            Console.WriteLine($"--> SaveDog method (EditDogViewModel)");
            //set the singlton object to mapin type 
            //var singleton = ObjectSingleton.Instance;
            //singleton.SetDogType();

            // Get the Realm instance
            var realm = RealmService.GetMainThreadRealm();

            //this check fixed the problem of no flexibale subscrption !!!!

            // Check if the subscription for Dog type exists
            var dogSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "DogSubscription");

            if (!dogSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for Dog. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var dogQuery = realm.All<Dog>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(dogQuery, new SubscriptionOptions { Name = "DogSubscription" });
                });

                Console.WriteLine("Dog subscription added. Waiting for synchronization...");

                // Wait for synchronization
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Subscriptions synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Dog subscription already exists.");
            }

            // Proceed with adding the Dog object
            await realm.WriteAsync(() =>
            {
                realm.Add(new Dog()
                {
                    OwnerId = RealmService.CurrentUser.Id,
                    Name = "summary",
                    Age = 5
                });
            });

            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }




        private async Task SavePin(Maui.GoogleMaps.Pin newPin, int pinNumber)
        {

            var realm = RealmService.GetMainThreadRealm();

            Console.WriteLine($"----->pinNumber -->'{pinNumber}");

            var mapPinSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "MapPinSubscription");

            if (!mapPinSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for MapPin. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var mapPinQuery = realm.All<MapPin>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(mapPinQuery, new SubscriptionOptions { Name = "MapPinSubscription" });
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



            Console.WriteLine($"----->pinNumber2222 -->'{pinNumber}");
            ObjectId newPinId;


            var pinAddingToMongo = new MapPin()
            {
                OwnerId = RealmService.CurrentUser.Id,
                Mapname = _inputTrackName,
                Labelpin = newPin.Label,
                Address = newPin.Address,
                Latitude = newPin.Position.Latitude.ToString(),
                Longitude = newPin.Position.Longitude.ToString()
            };


            await realm.WriteAsync(() =>
            {
                realm.Add(pinAddingToMongo); // Add the pin to Realm
    
            });



            Notify(); // Notify observers after saving a pin
        }


        private static ObjectId firstPinOfListAddedLast()
        {
            var singleton = TypeFactory.Instance;
            singleton.SetTrackMongoType();

            var realm = RealmService.GetMainThreadRealm();

            var latestMapPin = realm.All<MapPin>()
                .Where(p => p.OwnerId == RealmService.CurrentUser.Id)
                .OrderByDescending(p => p.Id) // Sort by ObjectId (newest first)
                .First(); // Get the most recent pin

            if (latestMapPin != null)
            {
                Console.WriteLine($"Most recent MapPin Id!!!: {latestMapPin.Id}");
                return latestMapPin.Id;

            }
            else
            {
                Console.WriteLine("No MapPins found for this user.");
            }


            return latestMapPin.Id;


        }

        private static string GetDateTime()
        {
            // Get the current date and time
            DateTime nowTime = DateTime.Now;
            string formattedTime = nowTime.ToString("HH:mm:ss ");

            DateTime nowDate = DateTime.Now;
            string formattedDate = nowDate.ToString("dd/MM/yyyy ");


            return formattedDate + formattedTime;
        }


        private static string GetCurrentDate()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedDate = now.ToString("dd/MM/yyyy ");



            return formattedDate;
        }

        public async Task SaveTrack(ObjectId firstPinIdNumber)
        {
            MapUtility mapUtility = new MapUtility(_pinsList);
            //AddRecordToDb getDate = new AddRecordToDb("GetDate");

            //var singleton = TypeFactory.Instance;
            //singleton.SetTrackMongoType();


            var realm = RealmService.GetMainThreadRealm();



            var trackSubscriptionExists = realm.Subscriptions.Any(sub => sub.Name == "Track1Subscription");

            if (!trackSubscriptionExists)
            {
                Console.WriteLine("No existing subscription for TrackMongo1. Adding one now...");

                // Add the subscription synchronously
                realm.Subscriptions.Update(() =>
                {
                    var trackQuery = realm.All<TrackMongo1>().Where(d => d.OwnerId == RealmService.CurrentUser.Id);
                    realm.Subscriptions.Add(trackQuery, new SubscriptionOptions { Name = "Track1Subscription" });
                });

                Console.WriteLine("Track subscription added. Waiting for synchronization...");

                // Wait for synchronization
                await realm.Subscriptions.WaitForSynchronizationAsync();
                Console.WriteLine("Track synchronized successfully.");
            }
            else
            {
                Console.WriteLine("Track subscription already exists.");
            }



            await realm.WriteAsync(() =>
            {


                realm.Add(new TrackMongo1()
                {
                    OwnerId = RealmService.CurrentUser.Id,
                    IdOfFirstPin = firstPinIdNumber,
                    TrackName = _inputTrackName,
                    DateOfCreation = GetDateTime(),
                    NumberOfPins = _pinsList.Count,
                    DistanceOfTrack = mapUtility.calculateTotalDistance(),
                });

            });

            Notify(); // Notify observers after saving a pin


        }



    }

    // Concrete Observer (Concrete implementation of Observer)
    public class TrackLogger : ITrackObserver
    {
        public void OnTrackUpdated(Track track)
        {
            Console.WriteLine("Track Updated: " + track.PinsList.Count + " pins.");
        }
    }
}
