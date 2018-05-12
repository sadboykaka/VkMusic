using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Messier16.Forms.Controls;
using System.Windows.Input;
using Algh.interfaces;


namespace VkMusic2.Views
{
    class DataTemplateAudio : DataTemplate
    {
        private static void ShowLyricPage(object o)
        {
            var e = o as IAudio;
            if (e == null) return;
            var p = ((App)Application.Current).lyricPage;
            p.Search(e, true);
            Application.Current.MainPage.Navigation.PushAsync(p);
        }

        public DataTemplateAudio(bool checkboxes) : base(() =>
        {
            var sl = new StackLayout
            {

                Padding = new Thickness(5, 0),
                Orientation = StackOrientation.Horizontal
            };

            if (checkboxes) sl.Children.Add(new Checkbox { IsVisible = true });
            sl.Children.Add(new AudioInfoView());

            var cl = new ViewCell { View = sl };

            var lyricAction = new MenuItem { Text = "Текст", Command = new Command(ShowLyricPage)  };
            lyricAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));


            cl.ContextActions.Add(lyricAction);

            return cl;
        })
        {
            
        }
    }
}
