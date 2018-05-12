using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;


namespace Algh.interfaces
{
    public interface ITrackManager
    {
        ObservableCollection<IAudio> Downloads { get;}

        void LoadTrack(IAudio tr, ObservableCollection<IAudio> tl);
        IEnumerable<IAudio> GetTracks();

        void DeleteTracks(IEnumerable<IAudio> trs);
    }
}
