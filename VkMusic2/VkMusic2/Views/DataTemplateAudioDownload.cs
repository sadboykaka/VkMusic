using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace VkMusic2.Views
{
    class DataTemplateAudioDownload : DataTemplate
    {

        static ImageSource DownloadPicSource = ImageSource.FromFile("ic_download_black_36dp.png");

        private static void ClickLoad(object sender, EventArgs e)
        {
            ((View)sender).IsVisible = false;
        }

        public DataTemplateAudioDownload(EventHandler download) : base(() =>
        {

        EventHandler[] Events = { ClickLoad, download };

        ClickableIcon load = new ClickableIcon(DownloadPicSource, Events);

            var sl = new StackLayout
            {

                Padding = new Thickness(5, 0),
                Orientation = StackOrientation.Horizontal
            };

            
            sl.Children.Add(load);
            sl.Children.Add(new AudioInfoView());

            return new ViewCell { View = sl };
        })
        {

        }
    }
}
