using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Services;
using AerobicWithMe.Interfaces;

using AerobicWithMe.ViewModels;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using System.Numerics;

namespace AerobicWithMe.Models
{


    // Concrete Subject (Concrete implementation of Subject)

    public partial class Track : ITrackSubject
    {
        private List<ITrackObserver> _observers = new List<ITrackObserver>();

        private List<Maui.GoogleMaps.Pin> _pinsList;
        private string _inputTrackName;







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

            Notify(); // Notify observers after upload
        }

        public async Task RemoveTrack(MapPin pinOfChosenMap)
        {

            string trackNameToDelete = pinOfChosenMap.Mapname;

            SetObjectToUpload UploadObjectToMongo = new SetObjectToUpload();

            //var singleton = SetObjectToUpload.Instance;
            UploadObjectToMongo.CreateMapPin();
            //singleton.SetMapPinType();
            var realm = RealmService.GetMainThreadRealm();

            await DeleteTrack(pinOfChosenMap);

            var mapToDelete = realm.All<MapPin>()
                .Where(track => track.Mapname == trackNameToDelete)
                .ToList();

            // Delete the mappins object from mongo 
            foreach (var pin in mapToDelete)
            {
                await DeleteSinglePin(pin);
            }
            Notify(); // Notify observers after deletion
        }

        //Delete single MapPin object from Mongodb
        private async Task DeleteSinglePin(MapPin pin)
        {

            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetMapPinType();

            SetObjectToUpload UploadObjectToMongo = new SetObjectToUpload();
            UploadObjectToMongo.CreateMapPin();

            var realm = RealmService.GetMainThreadRealm();
            await realm.WriteAsync(() => realm.Remove(pin));
        }

        //Delete TrackMongo1 objet from Mongodb 
        private async Task DeleteTrack(MapPin pin)
        {

            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetTrackMongoType();
            
            SetObjectToUpload UploadObjectToMongo = new SetObjectToUpload();
            UploadObjectToMongo.CreateTrackMongo();

            var realm = RealmService.GetMainThreadRealm();

            ObjectId pinId = pin.Id; // Replace with your actual MapPin Id
            
            var track = realm.All<TrackMongo1>()//Get Track object
            .FirstOrDefault(t => t.IdOfFirstPin == pinId);

            if (track == null)
                return;


            await realm.WriteAsync(() => realm.Remove(track));

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


            var pinAddingToMongo = new MapPin()
            {
                OwnerId = RealmService.CurrentUser.Id,
                Mapname = _inputTrackName,
                Labelpin = newPin.Label,
                Address = newPin.Address,
                Latitude = newPin.Position.Latitude.ToString(),
                Longitude = newPin.Position.Longitude.ToString()
            };
            if (pinNumber == 1)//get the first pin id that is added to mongodb 
                await SaveTrack(pinAddingToMongo.Id);

            await realm.WriteAsync(() =>
            {
                realm.Add(pinAddingToMongo); // Add the pin to Realm
    
            });



            Notify(); // Notify observers after saving a pin
        }





    

        public async Task SaveTrack(ObjectId firstPinIdNumber)
        {
            MapUtility mapUtility = new MapUtility(_pinsList);


            //var singleton = SetObjectToUpload.Instance;
            //singleton.SetTrackMongoType();


            SetObjectToUpload UploadObjectToMongo = new SetObjectToUpload();
            UploadObjectToMongo.CreateTrackMongo();

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
                    DateOfCreation = mapUtility.getDateTime(),
                    NumberOfPins = _pinsList.Count,
                    DistanceOfTrack = mapUtility.calculateTotalDistance(),
                });

            });

            Notify(); // Notify observers after saving a pin


        }



    }



    // part of the Observor Design Pattern-->Concrete Observer (Concrete implementation of Observer)
    public class TrackLogger : ITrackObserver
    {
        public void OnTrackUpdated(Track track)
        {
            Console.WriteLine("Track Updated: " + track.PinsList.Count + " pins.");
        }
    }
}
