using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;
using AerobicWithMe.Interfaces;


namespace AerobicWithMe.FiltersAndSorting
{
    public class SortByProfileName : IUserRecordSorter
    {
        public IQueryable<UserRecord> Sort(IQueryable<UserRecord> records)
        {
            return records.OrderBy(record => record.ProfileName);
        }
    }
}
