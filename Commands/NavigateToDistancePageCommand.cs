using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AerobicWithMe.Views;
using AerobicWithMe.Interfaces;
using AerobicWithMe.Models;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace AerobicWithMe.Commands
{
    public class NavigateToDistancePageCommand : I_Command
    {


        private readonly Maui.GoogleMaps.Map currentMap;
        private MapUtility MapHelperObject; // Declare m as a class-level variable

        public NavigateToDistancePageCommand(Maui.GoogleMaps.Map mapService)
        {
            this.currentMap = mapService;
        }


        public async Task Execute()
        {

            int numOfPins = currentMap.Pins.ToList().Count;
            MapHelperObject = new MapUtility(currentMap);
            Task<bool> enoughPins = MapHelperObject.enoughPins(numOfPins);
            if (await enoughPins)
            {
                List<Maui.GoogleMaps.Pin> pinsList = currentMap.Pins.ToList();
                MapHelperObject.setPinsList(pinsList);
                double totalDistance = MapHelperObject.calculateTotalDistance();
                var currentDistancePage = new DistancePage(totalDistance, pinsList, MapHelperObject);
                await Shell.Current.Navigation.PushAsync(currentDistancePage);
            }


        }
    }
}
