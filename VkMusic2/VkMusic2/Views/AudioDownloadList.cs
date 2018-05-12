using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Algh.interfaces;

namespace VkMusic2.Views
{
    class AudioDownloadList : ListView
    {
        public AudioDownloadList(EventHandler download,ObservableCollection<IAudio> Tracks, ObservableCollection<IAudio> check, EventHandler f, bool ficon)
        {
            ItemsSource = Tracks;
            ItemTemplate = new DataTemplateAudioDownload(download, check, f, ficon);
            HasUnevenRows = true;
        }

    }
}
