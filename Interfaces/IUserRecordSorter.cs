using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;

namespace AerobicWithMe.Interfaces
{
    public interface IUserRecordSorter
    {
        IQueryable<UserRecord> Sort(IQueryable<UserRecord> records);
    }
}
