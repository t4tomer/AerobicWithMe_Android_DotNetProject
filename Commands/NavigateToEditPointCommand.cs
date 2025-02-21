using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Interfaces;
using AerobicWithMe.Views;


namespace AerobicWithMe.Commands
{
    public class NavigateToEditPointCommand : I_Command
    {
        private readonly Maui.GoogleMaps.Map currentMap;

        public NavigateToEditPointCommand(Maui.GoogleMaps.Map mapService)
        {
            currentMap = mapService;
        }

        public async Task Execute()
        {
            List<Maui.GoogleMaps.Pin> pinsList = currentMap.Pins.ToList();
            var EditPinAddrPage = new EditPinAddr(pinsList, currentMap);
            await Shell.Current.Navigation.PushAsync(EditPinAddrPage);

        }

    }

}
