using System;
using System.Collections.Generic;
using System.Text;

namespace Algh.interfaces
{
    public interface IEqualizer
    {
        IEnumerable<(string, string, short)> Bands { get; }
        IEnumerable<(string, short)> Presets { get; }

        EventHandler PresetChange { get; set; }

        (short, short) GetBandLevelRange(short id);

        short Preset { get; set; }

        void SetBandLevel(short id, short f);
        void SaveBands(string path);
        short GetBandLevel(short id);

    }
}
