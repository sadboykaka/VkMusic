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
                if (indx >= Tl.Length || indx < 0) return null;
                return Tl[indx];
            }
        }

        private int curid;
        private int curtl;

        public int Curtl {
            get { return curtl; }
            set
            {
                int count = Tl.Length;
                if (value >= Tl.Length) curtl = 0;
                else if (value < 0)
                {
                    if (count == 0) curtl = 0;
                    else curtl = count - 1;
                }
                else curtl = value;
            }
        }
        public int Curid {
            get { return curid; }
            set {
                var tl = this[curtl];
                if (tl == null) return;
                int count = Tl[curtl].Count;
                if (value >= count) curid = 0;
                else if (value < 0)
                {
                    if (count == 0) curid = 0;
                    else curid = count - 1;
                }
                else curid = value;
            }
        }

        private ObservableCollection<IAudio>[] Tl;

        public AndroidTrackList(int count)
        {
            curtl = -1;
            Tl = new ObservableCollection<IAudio>[count];
            for (int i = 0; i < count; i++) Tl[i] = new ObservableCollection<IAudio>();
        }
    }
}
