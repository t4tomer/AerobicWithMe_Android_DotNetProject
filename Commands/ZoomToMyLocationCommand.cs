
using AerobicWithMe.Interfaces;

using Position = Maui.GoogleMaps.Position;


namespace AerobicWithMe.Commands
{
    public class ZoomToMyLocationCommand : I_Command
    {
        private readonly Maui.GoogleMaps.Map currentMap;
        private CancellationTokenSource _cancelTokenSource;

        private bool _isCheckingLocation;

        public ZoomToMyLocationCommand(Maui.GoogleMaps.Map mapService)
        {
            currentMap = mapService;
        }

        public async Task Execute()
        {
            await GetCurrentLocation();
        }


        public async Task<Location> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    CenterMap(location.Latitude, location.Longitude);

                }

                return location;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        private void CenterMap(double x, double y)
        {
            //var position = new Position(31.268333463883636, 34.80691033370654);
            var position = new Position(x, y);
            var mapSpan = Maui.GoogleMaps.MapSpan.FromCenterAndRadius(position, Maui.GoogleMaps.Distance.FromMeters(1)); // Adjust the radius as needed
            currentMap.MoveToRegion(mapSpan);
        }

    }
}
