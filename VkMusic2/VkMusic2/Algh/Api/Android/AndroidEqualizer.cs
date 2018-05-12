using Algh.interfaces;
using Android.Media;
using Android.Media.Audiofx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Algh.Api
{
    class AndroidEqualizer : IEqualizer
    {

        protected Equalizer equalizer;
        public const string BaseName = "Equalizer.conf";

        public IEnumerable<(string, short)> Presets { get; }

        public IEnumerable<(string, string, short)> Bands { get; private set; }

        private short[] BandsSettings;
        private short curpreset;
        public EventHandler PresetChange { get; set; }


        public short Preset {
            get { return curpreset; }
            set { SetPreset(value); }
        }

        public AndroidEqualizer(MediaPlayer player, string settingsPath)
        {
            equalizer = new Equalizer(0, player.AudioSessionId);
            equalizer.SetEnabled(true);

            Presets = new List<(string, short)>();
            ((List<(string, short)>)Presets).Add(("User", -1));
            for (short i = 0; i < equalizer.NumberOfPresets; i++) ((List<(string, short)>)Presets).Add((equalizer.GetPresetName(i), i));

            BandsSettings = LoadBands(settingsPath);

            Bands = new List<(string, string, short)>();

            for (short i = 0; i < equalizer.NumberOfBands; i++) ((List<(string, string, short)>)Bands).Add((equalizer.GetCenterFreq(i) + "Hz", "Db", i));

            SetPreset(-1);
        }

        virtual protected short[] LoadBands(string path)
        {
            try
            {
                using (System.IO.Stream stream = File.Open(path + "/" + BaseName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (short[])formatter.Deserialize(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        private void SetBandSettings()
        {
            try
            {
                BandsSettings = new short[equalizer.NumberOfBands];
                for (short i = 0; i < equalizer.NumberOfBands; i++) BandsSettings[i] = equalizer.GetBandLevel(i);
                return;
            }
            catch
            {
                return;
            }
        }

        virtual public void SaveBands(string path)
        {
            SetBandSettings();
            for (short i = 0; i < equalizer.NumberOfBands; i++) BandsSettings[i] = equalizer.GetBandLevel(i);
            using (System.IO.Stream stream = File.Create(path + "/" + BaseName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, BandsSettings);
            }
        }

        ~AndroidEqualizer()
        {
            equalizer.Release();
        }



        private void SetPreset(short id)
        {
            if (id == -1)
            {
                if (BandsSettings == null || BandsSettings.Length != equalizer.NumberOfBands)
                {
                    equalizer.UsePreset(0);
                    curpreset = 0;
                }
                else
                {
                    for (short i = 0; i < BandsSettings.Length; i++) equalizer.SetBandLevel(i, BandsSettings[i]);
                    curpreset = -1;
                }
                if (PresetChange != null)  PresetChange(this, new EventArgs());
                return;
            }
            if (equalizer.NumberOfPresets <= id) return;
            equalizer.UsePreset(id);
            curpreset = id;
            if(PresetChange != null) PresetChange(this, new EventArgs());
        }

        public void SetBandLevel(short id, short f)
        {
            if (id >= equalizer.NumberOfBands) return;
            
            equalizer.SetBandLevel(id, f);
        }

        public (short, short) GetBandLevelRange(short id)
        {
            short[] res = equalizer.GetBandLevelRange();
            return (res[0], res[1]);
        }

        public short GetBandLevel(short id)
        {
            if (id >= equalizer.NumberOfBands) return 0;
            
            return equalizer.GetBandLevel(id);
        }


      
    }
}
