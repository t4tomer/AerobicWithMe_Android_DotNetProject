using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// part of the Observor Design Pattern--> Subject block 

namespace AerobicWithMe.Interfaces
{
    public interface ITrackSubject
    {
        void Attach(ITrackObserver observer);
        void Detach(ITrackObserver observer);
        void Notify();
    }
}
