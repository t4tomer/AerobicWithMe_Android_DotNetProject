using System;

namespace AerobicWithMe.Models
{
    public class ObjectMongoFactory
    {
        private object _currentObject;

        // Constructor initializes with a default type
        public ObjectMongoFactory()
        {
            _currentObject = new MapPin();
            Console.WriteLine("Default object type is MapPin.");
        }

        // Method to create a MapPin instance
        public void CreateMapPin()
        {
            _currentObject = new MapPin();
            Console.WriteLine("Object type set to MapPin.");
        }

        // Method to create a UserRecord instance
        public void CreateUserRecord()
        {
            _currentObject = new UserRecord();
            Console.WriteLine("Object type set to UserRecord.");
        }

        // Method to create a TrackMongo instance
        public void CreateTrackMongo()
        {
            _currentObject = new TrackMongo1();
            Console.WriteLine("Object type set to TrackMongo.");
        }

        // Method to get the current object type
        public Type GetCurrentObjectType()
        {
            return _currentObject.GetType();
        }

        // Method to retrieve the current object
        public object GetCurrentObject()
        {
            return _currentObject;
        }
    }
}
