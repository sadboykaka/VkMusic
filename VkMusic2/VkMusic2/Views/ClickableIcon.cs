using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;

namespace VkMusic2.Views
{
    class ClickableIcon : Image
    {
        public ClickableIcon(ImageSource source, EventHandler eventclick)
        {
            Source = source;

            var iconTap = new TapGestureRecognizer();

            iconTap.Tapped += eventclick;

            this.GestureRecognizers.Add(iconTap);
        }

        public ClickableIcon(ImageSource source, IEnumerable<EventHandler> eventclicks)
        {
            Source = source;
            var iconTap = new TapGestureRecognizer();

            foreach (EventHandler eventclick in eventclicks) iconTap.Tapped += eventclick;

            this.GestureRecognizers.Add(iconTap);

        }

    }


    class ClickableHashIcon : FFImageLoading.Forms.CachedImage
    {
        public ClickableHashIcon(ImageSource source, EventHandler eventclick)
        {
            Source = source;

            var iconTap = new TapGestureRecognizer();

            iconTap.Tapped += eventclick;

            this.GestureRecognizers.Add(iconTap);
            

        }

        public ClickableHashIcon(ImageSource source, IEnumerable<EventHandler> eventclicks)
        {
            Source = source;
            DownsampleToViewSize = true;
            var iconTap = new TapGestureRecognizer();

            foreach (EventHandler eventclick in eventclicks) iconTap.Tapped += eventclick;

            this.GestureRecognizers.Add(iconTap);

        }

    }
}
