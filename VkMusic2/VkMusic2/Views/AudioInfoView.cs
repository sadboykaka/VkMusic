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

        public AudioInfoView()
        {
            Padding = pad;
            Orientation = StackOrientation.Vertical;
            NameLabel.SetBinding(Label.TextProperty, "Name");
            AuthorLabel.SetBinding(Label.TextProperty, "Author");
            Children.Add(NameLabel);
            Children.Add(AuthorLabel);
        }
        
    }
}
