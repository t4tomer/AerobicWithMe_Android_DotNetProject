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
            MapHelperObject = new MapUtility(currentMap);
           Task <bool> enoughPins = MapHelperObject.enoughPins(numOfPins);

            if (await enoughPins)
            {
                List<Maui.GoogleMaps.Pin> pinsList = currentMap.Pins.ToList();
                var AddToCloud = new AddMapToDbPage(pinsList, currentMap);
                await Shell.Current.Navigation.PushAsync(AddToCloud);

            }
        }




    }
}
