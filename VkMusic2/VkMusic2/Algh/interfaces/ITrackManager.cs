using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Algh.interfaces
{
    interface ITrackManager
    {
        Task<IAudio> LoadTrack(IAudio audio);
        IEnumerable<IAudio> GetTracks();

        void DeleteTracks(IEnumerable<IAudio> trs);
    }
}
