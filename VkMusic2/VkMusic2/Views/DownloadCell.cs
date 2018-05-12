using Algh.interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace VkMusic2.Views 
{
    class DownloadCell : ViewCell
    {
        private static void ClickLoad(object sender, EventArgs e)
        {

            ((View)sender).IsVisible = false;
        }

        static ImageSource DownloadPicSource = ImageSource.FromFile("ic_download_black_36dp.png");
        static ImageSource AddPicSource = ImageSource.FromFile("ic_plus_black_36dp.png");
        static ImageSource RemovePicSource = ImageSource.FromFile("ic_minus_black_36dp.png");

        ClickableIcon loadIcon;
        ClickableIcon addIcon;

        /// <summary>
        /// Download Cell Constructor
        /// </summary>
        /// <param name="download">Download Click Event.</param>
        /// <param name="check">Collection for check is track already downloaded.</param>
        /// <param name="add">Add\Remove Click event</param>
        /// <param name="icon">Add\Remove icon (true - plus (add), false - munus (remove))</param>
        public DownloadCell(EventHandler download, ObservableCollection<IAudio> check, EventHandler add, bool icon)
        {
            EventHandler[] EventsLoad = { ClickLoad, download };
            EventHandler[] EventsAdd = { ClickLoad, add };
            var sl = new StackLayout
            {

                Padding = new Thickness(5, 0),
                Orientation = StackOrientation.Horizontal
            };

            loadIcon = new ClickableIcon(DownloadPicSource, EventsLoad);
            addIcon = new ClickableIcon(icon ? AddPicSource : RemovePicSource, EventsAdd);

            sl.Children.Add(new AudioInfoView());

            var IconMenu = new StackLayout { Children = { addIcon, loadIcon }, HorizontalOptions = LayoutOptions.EndAndExpand, Orientation = StackOrientation.Horizontal  };
            sl.Children.Add(IconMenu);

            BindingContextChanged += (i, e) => {
                if (BindingContext == null) return;
                var res = ((IAudio)BindingContext);
                if (check.FirstOrDefault(x => x.ID == res.ID) != null) loadIcon.IsVisible = false;
            };
            
            

            View = sl;
        }
    }
}
