using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Messier16.Forms.Controls;

namespace VkMusic2.Views
{
    class DataTemplateAudio : DataTemplate
    {
        public DataTemplateAudio(bool checkboxes) : base(() =>
        {
            var sl = new StackLayout
            {

                Padding = new Thickness(5, 0),
                Orientation = StackOrientation.Horizontal
            };

            if (checkboxes) sl.Children.Add(new Checkbox { IsVisible = true });
            sl.Children.Add(new AudioInfoView());

            return new ViewCell { View = sl };
        })
        {

        }
    }
}
