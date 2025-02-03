using System;
using System.ComponentModel;
using System.Timers;  // Disambiguate Timer reference
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.Maui.Controls;

namespace AerobicWithMe.Views
{
    public partial class TimerPage : ContentPage, INotifyPropertyChanged
    {
        private static TimerPage _instance; // Singleton instance
        private static readonly object _lock = new object(); // Thread-safety lock
        private string _mapTitle = "Test"; // Default value

        private System.Timers.Timer _timer;
        private TimeSpan _elapsedTime;
        private bool isRunning = false;

        private string _timerText = "00:00:00"; // Backing field for TimerText

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged(nameof(TimerText)); // Notify the UI when the value changes
            }
        }


        public string TrackNameTiltle //XAML lavbel of the Title 
        {
            get => $"Timer for {_mapTitle} map";
            set
            {
                if (_mapTitle != value)
                {
                    _mapTitle = value;
                    OnPropertyChanged(nameof(TrackNameTiltle)); // Notify the UI about the change
                }
            }
        }


        public void setTitle(string newTitle)
        {
            TrackNameTiltle = newTitle; // Update property
            BindingContext = null;
            BindingContext = this; // Reset BindingContext to refresh bindings
        }




        // Private constructor to prevent direct instantiation
        private TimerPage()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer(1000);  // 1-second interval
            _timer.Elapsed += OnTimerElapsed;
            BindingContext = this;  // Set binding context to the page itself
        }

        // Public static property to get the singleton instance
        public static TimerPage Instance
        {
            get
            {
                // Ensure thread-safe access
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TimerPage();
                    }
                }
                return _instance;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!isRunning) return;

            _elapsedTime = _elapsedTime.Add(TimeSpan.FromSeconds(1));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText = _elapsedTime.ToString(@"hh\:mm\:ss"); // Update TimerText property
            });
            Console.WriteLine($"---->(OnTimerElapsed) Time:{_timerText}");
        }

        [RelayCommand]
        public void StartTimer()
        {
            if (!isRunning)
            {
                isRunning = true;
                _timer.Start();
            }
        }

        [RelayCommand]
        public void PauseTimer()
        {
            isRunning = false;
            _timer.Stop();
        }

        [RelayCommand]
        public void ResetTimer()
        {
            isRunning = false;
            _timer.Stop();
            _elapsedTime = TimeSpan.Zero;
            TimerText = "00:00:00"; // Reset TimerText
        }

        public event PropertyChangedEventHandler PropertyChanged;


        [RelayCommand]

        public async Task Upload()
        {
            Console.WriteLine($"---------> Upload record");
            PauseTimer();
            string newRecordTime = _timerText;//save the user record time 
            var addNewUserRecordToDb = new AddRecordToDb();
            addNewUserRecordToDb.setRecordUserTime(newRecordTime);
            addNewUserRecordToDb.setTrackName(_mapTitle);

            await Navigation.PushAsync(addNewUserRecordToDb);


        }


        private static string GetCurrentDateTime()
        {
            // Get the current date and time
            DateTime now = DateTime.Now;

            // Format it as a string
            //string formattedDateTime = now.ToString("yyyy-MM-dd HH:mm:ss");
            string formattedDateTime = now.ToString("dd-MM-yyyy HH:mm:ss");


            return formattedDateTime;
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
