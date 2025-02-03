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

namespace AerobicWithMe.Views
{

    //UserDetails
    //UserDetails

    public partial class UserDetails : ContentPage
    {

        private string _nullString = ""; //
        private UserRecord _currentUserRecord = null;


        public UserDetails()
        {
            InitializeComponent();
            BindingContext = this; // Set the BindingContext to the current page
        }

        public void setXAML_Values(UserRecord newUserRecord)
        {
            _currentUserRecord = newUserRecord;
            RecordUserProfileNameXAML = _currentUserRecord.ProfileName; // This will automatically update the UI
            RecordUserDateAndTimeXAML = _currentUserRecord.UploadDateTime;
            RecordUserTrackTimeXAML = _currentUserRecord.TrackTime;
            RecordUserTrackTrackNameXAML = _currentUserRecord.MapName;
            RecordUserTrackCommentXAML = _currentUserRecord.Comment;

        }

  

        public string RecordUserTrackCommentXAML
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(RecordUserTrackCommentXAML)); // Notify the UI about the change
                }
            }
        }




        public string RecordUserTrackTrackNameXAML
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(RecordUserTrackTrackNameXAML)); // Notify the UI about the change
                }
            }
        }



        public string RecordUserTrackTimeXAML
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(RecordUserTrackTimeXAML)); // Notify the UI about the change
                }
            }
        }


        public string RecordUserDateAndTimeXAML
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(RecordUserDateAndTimeXAML)); // Notify the UI about the change
                }
            }
        }




        public string RecordUserProfileNameXAML
        {
            get => _nullString;
            set
            {
                if (_nullString != value)
                {
                    _nullString = value;
                    OnPropertyChanged(nameof(RecordUserProfileNameXAML)); // Notify the UI about the change
                }
            }
        }




  
 




    }

}