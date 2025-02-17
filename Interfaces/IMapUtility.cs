using Maui.GoogleMaps;
using System.Collections.Generic;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Graphics;

namespace AerobicWithMe.Interfaces
{
    public interface IMapUtility
    {
        void addPointsToTrack();
        void addPointsToTrackOnMap();
        void setPinsList(List<Pin> newPinsList);
        void drawLineBetweenAllPins(int strokeColorPolyline);
        void deleteLastPoint(List<Pin> pinsList);
        List<string> getPtrNamesAndPolylines();

        public string getDateTime();

        public string getCurrentTime();
        public string getCurrentDate();


    }
}
