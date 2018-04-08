using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Algh.interfaces
{
    public interface ITrackList
    {
        int Curtl { get; set; }
        int Curid { get; set; }
        ObservableCollection<IAudio> this[int indx]
        {
            get;
        }
      

    }
}
