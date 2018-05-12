using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;

namespace Algh.interfaces
{
    public interface IPlayer
    {
        void Play(int id, int tlid);
        void PlayNext();
        void PlayPrev();
        void Pause();
        void PauseOrResume();
        void Stop();
        int Seek { get; set; }
        int Duration { get; }

        bool IsPlay { get; }
        bool Repeat { get; set; }
        ITrackList TrackList { get; }

        IEqualizer Equalizer { get; }

        event EventHandler<bool> PlayEvent;
        event EventHandler<int> SeekEvent;

    }
}
