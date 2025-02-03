using System;

namespace AerobicWithMe.Models
{
    public sealed class ObjectSingleton
    {
        private static readonly Lazy<ObjectSingleton> _instance = new(() => new ObjectSingleton());

        private object _currentType;

        // Private constructor to prevent instantiation
        private ObjectSingleton()
        {
            // Default type is MapPin
            _currentType = new MapPin();
            Console.WriteLine("Default object type is MapPin.");
        }

        // Public property to get the singleton instance
        public static ObjectSingleton Instance => _instance.Value;




        // Method to set type to MapPin
        public void SetMapPinType()
        {
            _currentType = new MapPin();
            Console.WriteLine("Object type set to MapPin.");
        }

        // Method to set type to MapPin
        public void SetUserRecordType()
        {
            _currentType = new UserRecord();
            Console.WriteLine("Object type set to UserRecord.");
        }


        // Method to get the current type
        public Type GetCurrentType()
        {
            return _currentType.GetType();
        }
    }


}
