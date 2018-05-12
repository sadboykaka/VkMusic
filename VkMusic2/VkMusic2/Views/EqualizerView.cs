using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Algh.interfaces;
using System.Linq;

namespace VkMusic2.Views
{
    class EqualizerView : StackLayout
    {

        List<(SliderWithDescription, short id)> sliders = new List<(SliderWithDescription, short id)>();
        Picker picker;
        IEqualizer Equalizer;
        private void InitBinds()
        {
            foreach (var a in Equalizer.Bands)
            {
                var range = Equalizer.GetBandLevelRange(a.Item3);
                var sl = new SliderWithDescription(a.Item1, a.Item2, range.Item1, range.Item2, Equalizer.GetBandLevel(a.Item3),
                    (i, e) => { Equalizer.SetBandLevel(a.Item3, (short)e); });
                this.Children.Add(sl);
                sliders.Add((sl, a.Item3));
            }
        }

        private void PresetChange(object i, EventArgs e)
        {
           foreach(var a in sliders) a.Item1.ChangeValue(Equalizer.GetBandLevel(a.Item2));
        }

        public EqualizerView(IEqualizer equalizer)
        {
            Equalizer = equalizer;

            Button button = new Button();
            button.Text = "Сохранить";
            button.Clicked += (i,e) => { equalizer.SaveBands(Algh.Api.HttpUser.Cookies.appdir); picker.SelectedIndex = 0; };

            StackLayout fother = new StackLayout();
            fother.Orientation = StackOrientation.Horizontal;

            picker = new Picker();

            short indx = 0;
            short slct = 0;
            foreach (var a in equalizer.Presets)
            {
                if (a.Item2 == equalizer.Preset) slct = indx;
                picker.Items.Add(a.Item1);
                indx++;
            }

            picker.SelectedIndex = slct;
            picker.SelectedIndexChanged += (i, e) => {
                var b = equalizer.Presets.FirstOrDefault(x => x.Item1 == picker.Items[picker.SelectedIndex]);
                equalizer.Preset = b.Item2;
            };

            fother.Children.Add(picker);
            fother.Children.Add(button);

            Children.Add(fother);

            InitBinds();
            equalizer.PresetChange += PresetChange;

            

        }
    }
}
