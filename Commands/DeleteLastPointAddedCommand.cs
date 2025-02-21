using AerobicWithMe.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using AerobicWithMe.Interfaces;

namespace AerobicWithMe.Commands
{
    public class DeleteLastPointAddedCommand : I_Command
    {
        private readonly Maui.GoogleMaps.Map currentMap;
        private MapUtility MapHelperObject; // Declare m as a class-level variable
        private List<Maui.GoogleMaps.Pin> pinsList;

        public DeleteLastPointAddedCommand(Maui.GoogleMaps.Map mapService)
        {
            currentMap = mapService;
            pinsList=currentMap.Pins.ToList();

            MapHelperObject = new MapUtility(pinsList, currentMap);


        }

        public async Task Execute()
        {
             MapHelperObject.deleteLastPoint(pinsList);
   
        }



 

    }
}
