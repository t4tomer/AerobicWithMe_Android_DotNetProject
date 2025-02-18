using DocumentFormat.OpenXml.Drawing.Diagrams;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Services;
using AerobicWithMe.Interfaces;

//object that is used for saving methods that are used for map page.

namespace AerobicWithMe.Models
{
    public  class MapUtility: IMapUtility
    {

        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        Maui.GoogleMaps.Map myMap;

        public MapUtility()
        {
            Console.WriteLine($"----> empty constructor MapHelper");
         
        }
        public MapUtility(List<Pin> pinsList)
        {
            this.pinsList = pinsList;
        }

        public MapUtility( Maui.GoogleMaps.Map newMyMap)
        {
            
            this.myMap = newMyMap;
        }


        public MapUtility(List<Pin> NewPinsList, Maui.GoogleMaps.Map newMyMap)
        {
            Console.WriteLine($"---->  constructor -MapHelper(PinsList,myMap)");

            this.pinsList = NewPinsList;
            this.myMap = newMyMap;  
        }


        public void setMap(Maui.GoogleMaps.Map myMap)
        {
            this.myMap = myMap;
        }


        public void addPointsToTrack()
        {
            foreach (var pin in pinsList)
            {
                //Console.WriteLine($"Pin Summary: {pin.Label}");
                myMap.Pins.Add(pin);
            }
        }



        public  string getDateTime()
        {



            return getCurrentDate() + getCurrentTime();
        }


        public string getCurrentTime()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedTime = now.ToString("HH:mm:ss ");



            return formattedTime;
        }


        public  string getCurrentDate()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedDate = now.ToString("dd/MM/yyyy ");



            return formattedDate;
        }






        public void addPointsToTrackOnMap() //used to show the pins on the map 
        {
            foreach (var pin in pinsList)
            {
                // Assuming each pin has a 'Label' property that holds the summary
                Console.WriteLine($"Pin Summary: {pin.Label}");
                myMap.Pins.Add(pin);

            }
        }

        public void setPinsList(List<Maui.GoogleMaps.Pin> newpinstList)
        {
            this.pinsList=newpinstList;
        }
        public void drawLineBetweenAllPins(int strokeColorPolyline)
        {
            switch (pinsList)
            {
                case null:
                    break;

                case { Count: 0 }:
                    break;

                case { Count: 1 }:
                    break;

                case { Count: 2 }:
                    drawLineBetween2Pins(pinsList[0], pinsList[1], strokeColorPolyline);
                    break;

                default:
                    using (var enumerator = pinsList.GetEnumerator())
                    {
                        if (!enumerator.MoveNext()) return;

                        Maui.GoogleMaps.Pin currentPin = enumerator.Current;
                        Maui.GoogleMaps.Pin nextPin = null;

                        while (enumerator.MoveNext())
                        {
                            nextPin = enumerator.Current;


                            drawLineBetween2Pins(currentPin, nextPin, strokeColorPolyline);

                            currentPin = nextPin;
                        }

                    }
                    break;
            }
        }


        private void removePolylineBetweenPins( Maui.GoogleMaps.Pin pin1, Maui.GoogleMaps.Pin pin2)
        {




            // Create a list to store all polylines that match the removal criteria
            List<Polyline> polylinesToRemove = new List<Polyline>();
            foreach (var polyline in myMap.Polylines.ToList())
            {
                // Check if the polyline contains both positions of pin1 and pin2
                if (polyline.Positions.Count == 2 &&
                    polyline.Positions[0].Latitude == pin1.Position.Latitude &&
                    polyline.Positions[0].Longitude == pin1.Position.Longitude &&
                    polyline.Positions[1].Latitude == pin2.Position.Latitude &&
                    polyline.Positions[1].Longitude == pin2.Position.Longitude)
                {
         

                    polylinesToRemove.Add(polyline);  // Add matching polyline to the list
                }
            }
            // Remove all matching polylines from the map
            foreach (var polylineToRemove in polylinesToRemove)
            {
                myMap.Polylines.Remove(polylineToRemove);
            }
            polylinesToRemove = null; // Dereference the list


        }



        // used to delet the last point added on the map and the polylin that connected to this point
        public void deleteLastPoint(List<Maui.GoogleMaps.Pin> pinsList)
        {
            Console.WriteLine($" \t\t-->Number of pins(MapHelper):"+ pinsList.Count);
            switch (pinsList.Count)
            {
                case 1:
                    {
                        var firstPin = pinsList[0];
                        myMap.Pins.Remove(firstPin);
                        myMap.Pins.Clear(); // Clear the map from pins
                        break;
                    }
                case 2:
                    {
                        var beforeLastPin = pinsList[0];
                        var lastPin = pinsList[1];
            

                        // Remove the polyline between the last pin and the pin before last pin
                        removePolylineBetweenPins(beforeLastPin, lastPin);

                        // Remove the last pin from the map's Pins collection
                        myMap.Pins.Remove(lastPin);
                        break;
                    }
                default:
                    {
                        if (pinsList.Count > 2)
                        {

                            // Get the last pin & the pin before last pin in the list
                            var lastPin = pinsList[pinsList.Count - 1];
                            var beforeLastPin = pinsList[pinsList.Count - 2];

                            // Remove the polyline between the last pin and the pin before last pin
                            removePolylineBetweenPins(beforeLastPin, lastPin);

                            // Remove the last pin from the pinsList and the map's Pins collection
                            pinsList.Remove(lastPin);
                            myMap.Pins.Remove(lastPin);
                        }
                        else
                        {
                        }
                        break;
                    }
            }


        }

        public List<string>  getPtrNamesAndPolylines()
        {
            // Create a list of strings
            List<string> stringList = new List<string>();
            string str = "";

            switch (pinsList)
            {
                case null:
                    Console.WriteLine("----> no points");
                    break;

                case { Count: 0 }:
                    Console.WriteLine("----> no points");
                    break;

                case { Count: 1 }:
                    Console.WriteLine("----> first point, no line");
                    break;

                case { Count: 2 }:
                    double dist = getDistance2Points(pinsList[0], pinsList[1]);
                    str = $"\np{pinsList[0].Label}➔p{pinsList[1].Label}, distance is: {dist} km";
                    stringList.Add(str);
                    break;

                default:
                    using (var enumerator = pinsList.GetEnumerator())
                    {
                        if (!enumerator.MoveNext()) return new List<string>();

                        Maui.GoogleMaps.Pin currentPin = enumerator.Current;
                        Maui.GoogleMaps.Pin nextPin = null;

                        while (enumerator.MoveNext())
                        {
                            nextPin = enumerator.Current;
                            dist = getDistance2Points(currentPin, nextPin);
                            str = $"\np{currentPin.Label}➔p{nextPin.Label}, distance is: {dist} km";
                            stringList.Add(str);
                            Console.WriteLine(str);
                            currentPin = nextPin;
                        }
                    }
                    break;
            }
            
            return stringList;
        }



    

       


        private void drawLineBetween2Pins(Pin pin1, Pin pin2,int strokeColorPolyline)
        {

            // Check if the pins and their positions are not null
            if (pin1 == null || pin2 == null || pin1.Position == null || pin2.Position == null)
            {
                Console.WriteLine("One or both pins or their positions are null.");
                return;
            }

            // Check if the map is initialized
            if (myMap == null)
            {
                Console.WriteLine("The map is not initialized.");
                return;
            }


            // Create a new polyline
            var polyline = new Polyline
            {
                StrokeColor = Colors.BlueViolet, // Set the color of the line
                StrokeWidth = 5,            // Set the width of the line
                Tag =pinsList.Count+1
            };


            // Add the positions from the pins to the polyline
            polyline.Positions.Add(pin1.Position);
            polyline.Positions.Add(pin2.Position);

            // Add the polyline to the map
            myMap.Polylines.Add(polyline);
        }




    // method that is used to print the total distance between all the points in the map 
    public double calculateTotalDistance()
        {

                double totalDistance = 0;
                if (pinsList == null || pinsList.Count == 0) return 0;

                if (pinsList.Count == 1)
                    Console.WriteLine($"----> first point,no distance");
                else if(pinsList.Count == 2)
                {
                   double dist= getDistance2Points(pinsList[0],pinsList[1]);
                   totalDistance = dist;
                

                }
                else
                {
                    totalDistance = 0;
                    using (var enumerator = pinsList.GetEnumerator())
                    {
                        if (!enumerator.MoveNext()) return 0;

                        Maui.GoogleMaps.Pin currentPin = enumerator.Current;
                        Maui.GoogleMaps.Pin nextPin = null;

                        while (enumerator.MoveNext())
                        {
                            nextPin = enumerator.Current;

                            Console.WriteLine($"Current Pin Address: '{currentPin.Label}': {currentPin.Address}, -> Next Pin Address: '{nextPin.Label}': {nextPin.Address}");

                            double dist= getDistance2Points(currentPin,nextPin);

                            totalDistance = totalDistance + dist;

                            currentPin = nextPin;
                        }

                        // For the last pin, nextPin will be null.
                        Console.WriteLine($"Current Pin Address: '{currentPin.Label}': {currentPin.Address} -> Next Pin Address: None");
                    }

                }

                Console.WriteLine($"the total distance is :{totalDistance} km");
                return totalDistance;

        }
    // calculate distance between 2 points on the map
        private double getDistance2Points(Maui.GoogleMaps.Pin p1, Maui.GoogleMaps.Pin p2)
        {
            // Access the Position property of each Pin
            var pos1 = p1.Position;
            var pos2 = p2.Position;

            // Convert Position to Location
            Location loc1 = new Location(pos1.Latitude, pos1.Longitude);
            Location loc2 = new Location(pos2.Latitude, pos2.Longitude);

            // Calculate the distance using Location.CalculateDistance
            double distance = Location.CalculateDistance(loc1, loc2, DistanceUnits.Kilometers);
            Console.WriteLine($"----> Distance between p1 and p2: {distance} km");
            
            double roundedDistance = Math.Round(distance, 2);

            return roundedDistance;
        }



      


    }
}
