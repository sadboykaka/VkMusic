using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Algh.interfaces;

namespace VkMusic2.Views
{
    class DataTemplateAudioDownload : DataTemplate
    {
        public DataTemplateAudioDownload(EventHandler download, ObservableCollection<IAudio> check, EventHandler f, bool ficon) : base(() =>
        {
            return new DownloadCell(download, check, f, ficon);
        })
        {
            
        }
    }
}
