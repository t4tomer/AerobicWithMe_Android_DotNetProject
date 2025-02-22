using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AerobicWithMe.Models;
// part of the Observor Design Pattern--> Observer block 
namespace AerobicWithMe.Interfaces
{
    public interface ITrackObserver
    {
        void OnTrackUpdated(Track track);
    }
}
