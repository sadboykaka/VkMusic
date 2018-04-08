using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace VkMusic2.Views
{
    class AudioListViewWithOutSelect : ListView
    {
        public AudioListViewWithOutSelect(ObservableCollection<Algh.interfaces.IAudio> Tracks)
        {
            ItemTemplate = new DataTemplateAudio(false);
            HasUnevenRows = true;

            ItemsSource = Tracks;
            ItemSelected += (i, e) => { SelectedItem = null; };
        }
    }
}
