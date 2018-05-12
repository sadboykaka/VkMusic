using Algh.interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VkMusic2.Views
{
    class AudioInfoView : StackLayout
    {
        Label NameLabel = new Label { FontSize = 18, Margin = new Thickness(15, 0, 0, 0) };
        
        Label AuthorLabel = new Label() { Margin = new Thickness(15, 0, 0, 0) };

        static Thickness pad = new Thickness(0, 5);

        static ImageSource defaultCover = ImageSource.FromFile("ic_music_grey600_36dp.png");

        public AudioInfoView()
        {
            Orientation = StackOrientation.Horizontal;
            Padding = pad;
            FFImageLoading.Forms.CachedImage Cover = new FFImageLoading.Forms.CachedImage();
            Cover.HeightRequest = 48;
            Cover.WidthRequest = 48;
            Cover.MinimumHeightRequest = 48;
            Cover.MinimumWidthRequest = 48;
            NameLabel.SetBinding(Label.TextProperty, "Name");
            AuthorLabel.SetBinding(Label.TextProperty, "Author");
            Children.Add(Cover);
            BindingContextChanged += (i, e) => {
                if (BindingContext == null) return;
                var res = ((IAudio)BindingContext);

                var cv = ((FFImageLoading.Forms.CachedImage)this.Children[0]);
                if (res.Cover == "") cv.Source = defaultCover;
                else cv.Source = ImageSource.FromUri(new Uri(res.Cover));
            };
            Children.Add(new StackLayout { Orientation = StackOrientation.Vertical, Children = { NameLabel, AuthorLabel } });
        }
        
    }
}
