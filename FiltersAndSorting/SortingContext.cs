using System.Linq;
using AerobicWithMe.Models;
using AerobicWithMe.Interfaces;

namespace AerobicWithMe.FiltersAndSorting
{
    public class SortingContext
    {
        private IUserRecordSorter _sorter;

        public SortingContext(IUserRecordSorter sorter)
        {
            _sorter = sorter;
        }

        public void SetSorter(IUserRecordSorter sorter)
        {
            _sorter = sorter;
        }

        public IQueryable<UserRecord> SortUsersRecords(IQueryable<UserRecord> userRecords)
        {
            return _sorter.Sort(userRecords);
        }
    }
}
