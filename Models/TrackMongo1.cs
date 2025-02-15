using Maui.GoogleMaps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AerobicWithMe.Models;
using AerobicWithMe.Services;
using AerobicWithMe.ViewModels;

using Position = Maui.GoogleMaps.Position;
using AerobicWithMe.Views; // Correct namespace for TestPage
using MongoDB.Bson;
using Realms;
using AerobicWithMe.Services;
using Realms.Sync;


namespace AerobicWithMe.Models
{
    public partial class TrackMongo1 : IRealmObject
    {

        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("Id_of_first_pin")]//The variable that appears on the MONGO DB website
        public ObjectId IdOfFirstPin { get; set; }


        [MapTo("TrackName")]//The variable that appears on the MONGO DB website
        [Required]
        public string TrackName { get; set; }

        [MapTo("DateOfCreationTheTrack")]//The variable that appears on the MONGO DB website
        [Required]
        public string DateOfCreation { get; set; }

        [MapTo("NumberOFPinsInTheTrack")]//The variable that appears on the MONGO DB website
        public int NumberOfPins { get; set; }


        [MapTo("DistanceOfTrackKm")]//The variable that appears on the MONGO DB website
        public double DistanceOfTrack { get; set; }


        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;



    }


}
