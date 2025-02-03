using AerobicWithMe.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace AerobicWithMe.Views
{
    public partial class DistancePage : ContentPage, INotifyPropertyChanged
    {
        private string _totalDistanceText;
        private List<string> _stringList;

        // This property is bound to the Label in XAML
        public string TotalDistanceText
        {
            get => _totalDistanceText;
            set
            {
                _totalDistanceText = value;
                OnPropertyChanged(nameof(TotalDistanceText)); // Notify the UI when the value changes
            }
        }

        // This property is bound to the ListView in XAML
        public List<string> StringList
        {
            get => _stringList;
            set
            {
                _stringList = value;
                OnPropertyChanged(nameof(StringList)); // Notify the UI when the value changes
            }
        }

        public DistancePage(double totalDistance, List<Maui.GoogleMaps.Pin> pinsList, MapHelper MapHelperObject)
        {
            InitializeComponent();

            TotalDistanceText = $"The total distance is: {totalDistance:F2} km";
            StringList = MapHelperObject.getPtrNamesAndPolylines(); // Retrieve and set the list of strings

            BindingContext = this; // Set the binding context to this page
        }
    }
}
