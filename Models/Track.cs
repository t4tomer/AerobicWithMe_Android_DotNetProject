using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using Realms;

namespace AerobicWithMe.Services
{
    // Observer Interface
    public interface ITrackObserver
    {
        void OnTrackUpdated(Track track);
    }

    // Subject Interface
    public interface ITrackSubject
    {
        void Attach(ITrackObserver observer);
        void Detach(ITrackObserver observer);
        void Notify();
    }

    public class Track : ITrackSubject
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
            foreach (var pin in _pinsList)
            {
                Console.WriteLine($"Uploading Pin -->'{pin.Label}': {pin.Address}");
                await SavePin(pin);
            }

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

        private async Task SavePin(Maui.GoogleMaps.Pin newPin)
        {
            var realm = RealmService.GetMainThreadRealm();
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

            Notify(); // Notify observers after saving a pin
        }
    }

    // Concrete Observer
    public class TrackLogger : ITrackObserver
    {
        public void OnTrackUpdated(Track track)
        {
            Console.WriteLine("Track Updated: " + track.PinsList.Count + " pins.");
        }
    }
}
