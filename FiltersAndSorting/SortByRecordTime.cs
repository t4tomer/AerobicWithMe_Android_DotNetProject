using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using AerobicWithMe.Interfaces;

namespace AerobicWithMe.FiltersAndSorting
{
    public class SortByRecordTime : IUserRecordSorter
    {




        public IQueryable<UserRecord> Sort(IQueryable<UserRecord> userRecordsList)
        {
            // Filter valid records and sort by parsed TimeSpan
            var sortedUserRecords = userRecordsList
                .AsEnumerable() // Move to in-memory processing
                .Where(record => IsValidTime(record.TrackTime))
                .OrderBy(record => TimeSpan.Parse(record.TrackTime))
                .AsQueryable(); // Convert back to IQueryable if needed

            return sortedUserRecords;
        }

        private static bool IsValidTime(string? trackTime)
        {
            return !string.IsNullOrEmpty(trackTime) && TimeSpan.TryParse(trackTime, out _);
        }
    }

}
