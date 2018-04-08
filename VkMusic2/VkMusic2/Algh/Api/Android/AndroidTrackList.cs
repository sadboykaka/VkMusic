using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Algh.interfaces;

namespace Algh.Api
{
    class AndroidTrackList : ITrackList
    {
        public ObservableCollection<IAudio> this[int indx]
        {
            get
            {
                if (indx >= Tl.Length) return null;
                return Tl[indx];
            }
        }

        public int Curtl { get; set; }
        public int Curid { get; set; }

        private ObservableCollection<IAudio>[] Tl;

        public AndroidTrackList(int count)
        {
            Curtl = -1;
            Tl = new ObservableCollection<IAudio>[count];
            for (int i = 0; i < count; i++) Tl[i] = new ObservableCollection<IAudio>();
        }
    }
}
