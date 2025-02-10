using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using AerobicWithMe.ViewModels;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage

using Realms.Sync;


namespace AerobicWithMe.Services
{
    public class Track
    {

        // Private field to store the pins list
        private List<Maui.GoogleMaps.Pin> _pinsList;
        private string _inputTrackName;
        private MapPin initialMapPin;


        // Public property with getter and setter for the pins list
        public List<Maui.GoogleMaps.Pin> PinsList
        {
            get => _pinsList;  // Getter returns the current list
            set => _pinsList = value; // Setter assigns the provided list
        }

        // Constructor to initialize MongoDB collection
        public Track(string NewInputTrackName, List<Maui.GoogleMaps.Pin> NewPinsList)
        {
            _pinsList = NewPinsList;
            _inputTrackName = NewInputTrackName;
        }

        public async Task UploadToMongoDb()
        {
            foreach (var pin in _pinsList)
            {
                Console.WriteLine($"PrintPinAddresses -->'{pin.Label}': {pin.Address}");
                await SavePin(pin);
            }
        }


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
                    realm.Subscriptions.Add(mapPinQuery, new SubscriptionOptions { Name = "TrackSubscription" });
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

                
                realm.Add(new MapPin()
                {
                    OwnerId = RealmService.CurrentUser.Id,
                    Mapname = _inputTrackName,
                    Labelpin = newPin.Label,
                    Address = newPin.Address,
                    Latitude = newPin.Position.Latitude.ToString(),
                    Longitude = newPin.Position.Longitude.ToString()
                });
                
            });




            Console.WriteLine($"To view your data in Atlas, use this link: {RealmService.DataExplorerLink}");
            await Shell.Current.GoToAsync("..");
        }
    }


}
