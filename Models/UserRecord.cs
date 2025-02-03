using MongoDB.Bson;
using Realms;
using AerobicWithMe.Services;

namespace AerobicWithMe.Models
{
    public partial class UserRecord : IRealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("owner_id")]
        [Required]
        public string OwnerId { get; set; }

        [MapTo("ProfileName")]//The variable that appears on the MONGO DB website
        [Required]
        public string ProfileName { get; set; }


        [MapTo("MapName")]//The variable that appears on the MONGO DB website
        [Required]

        public string MapName { get; set; }


        [MapTo("TrackTime")]//The variable that appears on the MONGO DB website
        [Required]
        public string TrackTime { get; set; }


        //The variable that appears on the MONGO DB website
        [MapTo("UploadDateTime")] // the date and time the user uploaded to mongodb.
        [Required]

        public string UploadDateTime { get; set; }


        [MapTo("Comment")] // user comments 
        [Required]

        public string Comment { get; set; }



        public bool IsMine => OwnerId == RealmService.CurrentUser.Id;
    }
}

