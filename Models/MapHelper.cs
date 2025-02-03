using DocumentFormat.OpenXml.Drawing.Diagrams;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;

//object that is used for saving methods that are used for map page.


namespace AerobicWithMe.Models
{
    public  class MapHelper
    {

        List<Maui.GoogleMaps.Pin> pinsList;// the list of pins in the map
        Maui.GoogleMaps.Map myMap;

        public MapHelper()
        {
            Console.WriteLine($"----> empty constructor MapHelper");
         
        }
        public MapHelper(List<Pin> pinsList)
        {
            this.pinsList = pinsList;
        }

        public MapHelper( Maui.GoogleMaps.Map newMyMap)
        {
            
            this.myMap = newMyMap;
        }


        public MapHelper(List<Pin> NewPinsList, Maui.GoogleMaps.Map newMyMap)
        {
            Console.WriteLine($"---->  constructor -MapHelper(PinsList,myMap)");

            this.pinsList = NewPinsList;
            this.myMap = newMyMap;  
        }


        public void setMap(Maui.GoogleMaps.Map myMap)
        {
            this.myMap = myMap;
        }


        public void showTrack()
        {
            foreach (var pin in pinsList)
            {
                // Assuming each pin has a 'Label' property that holds the summary
                Console.WriteLine($"Pin Summary: {pin.Label}");
                myMap.Pins.Add(pin);

            }
        }

        private Maui.GoogleMaps.Pin getPoint(double x, double y, string inputLabel, string inputAddress)
        {
            var position = new Location(x, y);
            var pin = new Maui.GoogleMaps.Pin
            {
                Label = inputLabel,
                Address = inputAddress,
                Type = PinType.Place,
                Position = new Position(position.Latitude, position.Longitude)
            };

            myMap.Pins.Add(pin);
            return pin;
        }




        public void showTrackOnMap() //used to show the pins on the map 
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

        //method used for testing-print pin address
        /*
        public void PrintPinAddresses()
        {
            if (pinsList != null)
            {
                int pinCount = pinsList.Count;
                Console.WriteLine($"number of pins in the list(PrintPinAddresses): -->'{pinCount}");

                foreach (var pin in pinsList)
                {
                    Console.WriteLine($"PrintPinAddresses -->'{pin.Label}': {pin.Address}");
                }
            }
            else
            {
                Console.WriteLine("pinsList is null in PrintPinAddresses(PrintPinAddresses)");
            }
        }


               public void PrintPinAddresses(object sender, MapClickedEventArgs e)
        {
            var pins = pinsList;
            foreach (var pin in pins)
            {
                Console.WriteLine($"Address of pin (MapHelper class) -->'{pin.Label}': {pin.Address}");
            }
        }



        */











        public void drawLineBetweenAllPins(int strokeColorPolyline)
        {
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

                            Console.WriteLine($"Current Pin Address: '{currentPin.Label}': {currentPin.Address}, -> Next Pin Address: '{nextPin.Label}': {nextPin.Address}");

                            drawLineBetween2Pins(currentPin, nextPin, strokeColorPolyline);

                            currentPin = nextPin;
                        }

                        // For the last pin, nextPin will be null.
                        Console.WriteLine($"Current Pin Address: '{currentPin.Label}': {currentPin.Address} -> Next Pin Address: None");
                    }
                    break;
            }
        }


        private void removePolylineBetweenPins( Maui.GoogleMaps.Pin pin1, Maui.GoogleMaps.Pin pin2)
        {


            Console.WriteLine($" \t\t-->pin1:" + pin1.Label);
            Console.WriteLine($" \t\t-->pin2:" + pin2.Label);


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
                Console.WriteLine($" \t\t-->Polyline removed.");
            }
            polylinesToRemove = null; // Dereference the list


        }



        // used to delet the last point added on the map and the polylin that connected to this point
        public void deleteLastPoint(List<Maui.GoogleMaps.Pin> pinsList)
        {
            //TODO fix the problem of removing last point with the polyline
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
                        Console.WriteLine($" \t\t-->lastPin: {lastPin.Label}");
                        Console.WriteLine($" \t\t-->beforeLastPin: {beforeLastPin.Label}");
                        Console.WriteLine($" \t\t-->pinsList.Count: {pinsList.Count}");

                        // Remove the polyline between the last pin and the pin before last pin
                        removePolylineBetweenPins(beforeLastPin, lastPin);

                        // Remove the last pin from the map's Pins collection
                        myMap.Pins.Remove(lastPin);
                        Console.WriteLine("There are 2 pins!!!");
                        break;
                    }
                default:
                    {
                        if (pinsList.Count > 2)
                        {
                            Console.WriteLine($" \t\t-->there are more than 2 pins: {pinsList.Count}");

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
                            Console.WriteLine("No pins to remove.");
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
                    Console.WriteLine(str+"TEST!!!");
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



        private Color[] GetAllColors()
        {
            return new Color[]
            {
                Colors.Aqua,
                Colors.Aquamarine,
                Colors.Azure,
                Colors.Beige,
                Colors.Bisque,
                Colors.Black,
                Colors.BlanchedAlmond,
                Colors.Blue,
                Colors.BlueViolet,
                Colors.Brown,
                Colors.BurlyWood,
                Colors.CadetBlue,
                Colors.Chartreuse,
                Colors.Chocolate,
                Colors.Coral,
                Colors.CornflowerBlue,
                Colors.Cornsilk,
                Colors.Crimson,
                Colors.Cyan,
                Colors.DarkBlue,
                Colors.DarkCyan,
                Colors.DarkGoldenrod,
                Colors.DarkGray,
                Colors.DarkGreen,
                Colors.DarkGrey,
                Colors.DarkKhaki,
                Colors.DarkMagenta,
                Colors.DarkOliveGreen,
                Colors.DarkOrange,
                Colors.DarkOrchid,
                Colors.DarkRed,
                Colors.DarkSalmon,
                Colors.DarkSeaGreen,
                Colors.DarkSlateBlue,
                Colors.DarkSlateGray,
                Colors.DarkSlateGrey,
                Colors.DarkTurquoise,
                Colors.DarkViolet,
                Colors.DeepPink,
                Colors.DeepSkyBlue,
                Colors.DimGray,
                Colors.DimGrey,
                Colors.DodgerBlue,
                Colors.Firebrick,
                Colors.ForestGreen,
                Colors.Fuchsia,
                Colors.Gainsboro,
                Colors.Gold,
                Colors.Goldenrod,
                Colors.Gray,
                Colors.Green,
                Colors.GreenYellow,
                Colors.Grey,
                Colors.Honeydew,
                Colors.HotPink,
                Colors.IndianRed,
                Colors.Indigo,
                Colors.Ivory,
                Colors.Khaki,
                Colors.Lavender,
                Colors.LavenderBlush,
                Colors.LawnGreen,
                Colors.LemonChiffon,
                Colors.LightBlue,
                Colors.LightCoral,
                Colors.LightCyan,
                Colors.LightGoldenrodYellow,
                Colors.LightGray,
                Colors.LightGreen,
                Colors.LightGrey,
                Colors.LightPink,
                Colors.LightSalmon,
                Colors.LightSeaGreen,
                Colors.LightSkyBlue,
                Colors.LightSlateGray,
                Colors.LightSlateGrey,
                Colors.LightSteelBlue,
                Colors.LightYellow,
                Colors.Lime,
                Colors.LimeGreen,
                Colors.Linen,
                Colors.Magenta,
                Colors.Maroon,
                Colors.MediumAquamarine,
                Colors.MediumBlue,
                Colors.MediumOrchid,
                Colors.MediumPurple,
                Colors.MediumSeaGreen,
                Colors.MediumSlateBlue,
                Colors.MediumSpringGreen,
                Colors.MediumTurquoise,
                Colors.MediumVioletRed,
                Colors.MidnightBlue,
                Colors.MintCream,
                Colors.MistyRose,
                Colors.Moccasin,
                Colors.Navy,
                Colors.OldLace,
                Colors.Olive,
                Colors.OliveDrab,
                Colors.Orange,
                Colors.OrangeRed,
                Colors.Orchid,
                Colors.PaleGoldenrod,
                Colors.PaleGreen,
                Colors.PaleTurquoise,
                Colors.PaleVioletRed,
                Colors.PapayaWhip,
                Colors.PeachPuff,
                Colors.Peru,
                Colors.Pink,
                Colors.Plum,
                Colors.PowderBlue,
                Colors.Purple,
                Colors.Red,
                Colors.RosyBrown,
                Colors.RoyalBlue,
                Colors.SaddleBrown,
                Colors.Salmon,
                Colors.SandyBrown,
                Colors.SeaGreen,
                Colors.SeaShell,
                Colors.Sienna,
                Colors.Silver,
                Colors.SkyBlue,
                Colors.SlateBlue,
                Colors.SlateGray,
                Colors.SlateGrey,
                Colors.SpringGreen,
                Colors.SteelBlue,
                Colors.Tan,
                Colors.Teal,
                Colors.Thistle,
                Colors.Tomato,
                Colors.Transparent,
                Colors.Turquoise,
                Colors.Violet,
            };
        }

        private Color[] GetRandomizedColors()
        {
            Color[] colors = GetAllColors(); // Retrieve the array of all colors
            Random random = new Random();

            // Fisher-Yates shuffle algorithm to randomize the array
            for (int i = colors.Length - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);
                // Swap the colors
                Color temp = colors[i];
                colors[i] = colors[randomIndex];
                colors[randomIndex] = temp;
            }

            return colors;
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

            //TODO return to this 
            Color[] colors = GetAllColors(); // Retrieve the array of all colors
            if (strokeColorPolyline == colors.Length)
                strokeColorPolyline = 0;

            //strokeColorPolyline = strokeColorPolyline + 1;
            Console.WriteLine("strokeColorPolyline-->:"+ strokeColorPolyline);
            Console.WriteLine("colors:" + colors[strokeColorPolyline]);
            // Create a new polyline
            var polyline = new Polyline
            {
                //TODO need to fix the the line
                // so that between every 2 points there will polyline withdiffrent color

                StrokeColor = colors[8], // Set the color of the line
                StrokeWidth = 5,            // Set the width of the line
                Tag =pinsList.Count+1
            };
            Console.WriteLine("strokeColorPolyline-->:" + strokeColorPolyline);


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


     // Add new point on the map -used for testing 
     /*
        public static void OnMapClicked666(object sender, MapClickedEventArgs e, Maui.GoogleMaps.Map myMap)
        {
            Console.WriteLine($"----> Pressed the screen666 ");

            var pin = new Pin
            {
                Label = "New Pin",
                Address = "Custom Address",
                Position = e.Point,
                Type = PinType.Place
            };

            myMap.Pins.Add(pin);
        }

        */


      
        // add new point on the map 
        private void addPointOnMap(object sender, MapClickedEventArgs e, Maui.GoogleMaps.Map myMap)
        {
            Console.WriteLine($"----> Added Point to screen ");

            var pin = new Maui.GoogleMaps.Pin
            {
                Label = "New Pin",
                Address = "Custom Address",
                Position = e.Point,
                Type = PinType.Place
            };

            myMap.Pins.Add(pin);
        }
        public static Pin getPoint(double x, double y, string inputLabel, string inputAddress, Maui.GoogleMaps.Map myMap)
        {
            var position = new Location(x, y);
            var pin1 = new Pin
            {
                Label = inputLabel,
                Address = inputAddress,
                Type = PinType.Place,
                Position = new Position(position.Latitude, position.Longitude)
            };
            myMap.Pins.Add(pin1);
            return pin1;
        }
    }
}
