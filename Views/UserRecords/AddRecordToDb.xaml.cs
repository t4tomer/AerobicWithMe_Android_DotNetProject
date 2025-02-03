using Microsoft.Maui.Controls;
using Maui.GoogleMaps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Position = Maui.GoogleMaps.Position;
using Microsoft.Maui.Controls.Maps;
using System.Net.NetworkInformation;
using AerobicWithMe.Models;
using AerobicWithMe.ViewModels;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using AerobicWithMe.Services;



namespace AerobicWithMe.Views
{

    public partial class AddRecordToDb : ContentPage
    {

        List<Maui.GoogleMaps.Pin> pinsList; // the list of pins in the map
        Maui.GoogleMaps.Map myMap;
        private string _recordUserTime = ""; //
        private string _trackName = ""; //show on XAML page
        private string _formattedTime = "";//show on XAML page
        private string _formattedDate = "";//show on XAML page

        private string _inputUserName;  // get entry from XAML page 
        private string _commentText;//get entry from XAML page
        public string InputUserName
        {
            get => _inputUserName;
            set
            {
                


                if (_inputUserName != value)
                {
                    _inputUserName = value;
                    OnPropertyChanged(nameof(InputUserName));  // Notify UI about the change
                }
            }
        }

        public string CommentText
        {
            get => _commentText;
            set
            {
                if (_commentText != value)
                {
                    _commentText = value;
                    OnPropertyChanged(nameof(CommentText));  // Notify UI about the change
                }
            }
        }

        public string CurrentTime
        {
            get => GetCurrentTime();
            set
            {
                if (_formattedTime != value)
                {
                    _formattedTime = value;
                    OnPropertyChanged(nameof(CurrentTime)); // Notify the UI about the change
                }
            }
        }

        public string CurrentDate
        {
            get => GetCurrentDate();
            set
            {
                if (_formattedDate != value)
                {
                    _formattedDate = value;
                    OnPropertyChanged(nameof(CurrentDate)); // Notify the UI about the change
                }
            }
        }


        private static string GetCurrentTime()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedTime = now.ToString("HH:mm:ss ");



            return formattedTime;
        }


        private static string GetCurrentDate()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            string formattedDate = now.ToString("dd/MM/yyyy ");



            return formattedDate;
        }


        public AddRecordToDb()
        {
            InitializeComponent();

            BindingContext = this; // Set the BindingContext to the current page

            // Navigate to the singleton instance of TimerPage
            //var timerPage = TimerPage.Instance;
            //timerPage.ResetTimer();

        }


        public void setRecordUserTime(string newRecordUserTime)
        {
            _recordUserTime = newRecordUserTime; // Update the internal mapTitle field

            BindingContext = null;
            BindingContext = this; // Reset BindingContext to refresh bindings

            Console.WriteLine($"the new record time is--->: {_recordUserTime}");
        }

        public void setTrackName(string newTrackName)
        {
            _trackName = newTrackName;

            BindingContext = null;
            BindingContext = this; // Reset BindingContext to refresh bindings



            Console.WriteLine($"---------> the track name is : {_trackName}");

        }

        public string RecordUserTime
        {
            get => _recordUserTime;
            set
            {
                if (_recordUserTime != value)
                {
                    _recordUserTime = value;
                    OnPropertyChanged(nameof(RecordUserTime)); // Notify the UI about the change
                }
            }
        }



        public string TrackName
        {

            get => _trackName;
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged(nameof(TrackName)); // Notify the UI about the change
                }
            }
        }


        public string TrackNameTitle
        {

            get => $"Map: {_trackName}";
            set
            {
                if (_trackName != value)
                {
                    _trackName = value;
                    OnPropertyChanged(nameof(TrackName)); // Notify the UI about the change
                }
            }
        }


        [RelayCommand]

        //upload new user record to mongodb 
        public async Task UpLoadToMongo()
        {

            if (string.IsNullOrEmpty(_inputUserName))
            {
                Console.WriteLine($"---------> empty string name");
                await DialogService.ShowAlertAsync("Error", "Can Not Enter Empty User Name.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(_commentText))
                _commentText = "No Comment Inserted";


                string InputUploadDateTime = CurrentDate + CurrentTime;
            Console.WriteLine($"the InputProfileName is---> :{InputUserName} ");

            
            Console.WriteLine($"the InputMapName is---> :{TrackName} ");

            Console.WriteLine($"the InputTrackTime is--->: {_recordUserTime}");

            Console.WriteLine($"the InputUploadDateTime is---> :{InputUploadDateTime} ");

            Console.WriteLine($"the InputCommentText is---> :{CommentText} ");

            // Create the UserRecord object
            var newUserRecord = new UserRecord
            {
                ProfileName = InputUserName,
                MapName = TrackName,
                TrackTime = _recordUserTime,
                UploadDateTime = InputUploadDateTime,
                Comment = CommentText
            };


            // add new user record to mongo db 
            EditUserRecordViewModel addToDb = new EditUserRecordViewModel();
            await addToDb.SaveUserRecord(newUserRecord);


        }



    }

}