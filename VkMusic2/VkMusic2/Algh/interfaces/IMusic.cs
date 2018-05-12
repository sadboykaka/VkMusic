using System;
using System.Collections.Generic;
using System.IO;

using System.Threading.Tasks;

namespace Algh.interfaces
{
    public interface IMusic
    {

        Task<IEnumerable<IAudio>> GetMusicFromSearch(string name, int offset);

        Task<IEnumerable<IAudio>> GetMyMusic(int offset);

        Task<Stream> LoadTrack(IAudio audio);
        Task<Stream> LoadCover(IAudio audio);

        void DeleteTrack(IAudio track);

        void AddTrack(IAudio track);
    }
}
