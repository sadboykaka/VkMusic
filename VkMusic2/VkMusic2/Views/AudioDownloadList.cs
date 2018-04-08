using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace VkMusic2.Views
{
    class AudioDownloadList : ListView
    {
        public AudioDownloadList(EventHandler download,ObservableCollection<Algh.interfaces.IAudio> Tracks)
        {
            ItemsSource = Tracks;
            ItemTemplate = new DataTemplateAudioDownload(download);
            HasUnevenRows = true;
        }

    }
}
