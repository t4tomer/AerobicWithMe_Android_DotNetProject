using AerobicWithMe.Interfaces;

using AerobicWithMe.Services;
using AerobicWithMe.Models;
using AerobicWithMe.Views;

using DocumentFormat.OpenXml.Drawing.Diagrams;



namespace AerobicWithMe.Commands
{
    public class NavigateToUploadTrackPageCommand : I_Command
    {

        private readonly Maui.GoogleMaps.Map currentMap;
        private MapUtility MapHelperObject; // Declare m as a class-level variable


        public NavigateToUploadTrackPageCommand(Maui.GoogleMaps.Map mapService)
        {
            currentMap = mapService;
        }
        public async Task Execute()
        {
            int numOfPins = currentMap.Pins.ToList().Count;
            if (await EnoughPins(numOfPins))
            {
                MapHelperObject = new MapUtility(currentMap);
                List<Maui.GoogleMaps.Pin> pinsList = currentMap.Pins.ToList();
                var AddToCloud = new AddMapToDbPage(pinsList, currentMap);
                await Shell.Current.Navigation.PushAsync(AddToCloud);

            }
        }


        private async Task<bool> EnoughPins(int num)
        {
            if (num == 0)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add at least 2 points).", "OK");
                return false;
            }
            else if (num == 1)
            {
                await DialogService.ShowAlertAsync("Error", "Not Enough Pins(add 1 more point).", "OK");
                return false;
            }
            return true;

        }


    }
}
