using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;

namespace AerobicWithMe.Views
{
    public partial class PropertyTriggerXaml : ContentPage
    {
        List<Maui.GoogleMaps.Pin> pinsList; // the list of pins in the map

        public PropertyTriggerXaml()
        {
            InitializeComponent();
        }

        public PropertyTriggerXaml(List<Maui.GoogleMaps.Pin> newPinsList)
        {
            InitializeComponent();
            SetPinsList(newPinsList);
        }

        public void SetPinsList(List<Maui.GoogleMaps.Pin> newPinsList)
        {
            this.pinsList = newPinsList;
            if (pinsList != null)
            {
                int pinCount = pinsList.Count;
                Console.WriteLine($"number of pins in the list(TEST!!!): -->'{pinCount}': {pinCount}");
            }
            else
            {
                Console.WriteLine("pinsList is null(SetPinsList)");
            }
        }

        public void PrintPinAddresses()
        {
            if (pinsList != null)
            {
                int pinCount = pinsList.Count;
                Console.WriteLine($"number of pins in the list(PrintPinAddresses): -->'{pinCount}': {pinCount}");

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

        public Maui.GoogleMaps.Pin getPin(string pinLabel)
        {
            
            foreach (var pin in pinsList)
            {
                if (pin.Label == pinLabel)
                {
                    Console.WriteLine($"--->The '{pinLabel} exists in the pins list");
                    return pin;
                }

            }
            return null;

        }


        public bool PinExists(string pinLabel)
        {
            if (pinsList != null)
            {
                foreach (var pin in pinsList)
                {
                    if (pin.Label == pinLabel)
                    {
                        Console.WriteLine($"--->The '{pinLabel} exists in the pins list");
                        return true;
                    }                  

                }
            }
            Console.WriteLine($"--->The '{pinLabel} does not exists in the pins list");       
            return false;
        }

        private void setAddress(string pinLabel,string newAddress)
        {
            foreach (var pin in pinsList)
            {
                if (pin.Label == pinLabel)
                {

                    pin.Address = newAddress;

     
                }

            }

        }






        private void OnDoneButtonClicked(object sender, EventArgs e)
        {
            if (pinsList != null)
            {

                //int pinCount = pinsList.Count;
                //Console.WriteLine($"number of pins in the list(OnDoneButtonClicked): -->'{pinCount}': {pinCount}");
                string pinLabel = pinLabelEntry.Text;
                if (PinExists(pinLabel))
                {
                    string pinAddress = pinAddressEntry.Text;
                    Console.WriteLine($"(OnDoneButtonClicked)Entered label: {pinLabel}, changing: {pinAddress}");
                    setAddress(pinLabel, pinAddress);

                }


            }
            else
            {
                Console.WriteLine("pinsList is null in OnDoneButtonClicked");
            }
        }
    }
}
