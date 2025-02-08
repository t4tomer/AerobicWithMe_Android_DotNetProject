using System;

namespace AerobicWithMe.Models
{
    public sealed class TypeFactory
    {
        private static readonly Lazy<TypeFactory> _instance = new(() => new TypeFactory());

        private object _currentType;

        // Private constructor to prevent instantiation
        private TypeFactory()
        {
            // Default type is MapPin
            _currentType = new MapPin();
            Console.WriteLine("Default object type is MapPin.");
        }

        // Public property to get the singleton instance
        public static TypeFactory Instance => _instance.Value;




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
