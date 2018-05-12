using System;
using System.Text.RegularExpressions;
using Algh.interfaces;
using Xamarin.Forms;
namespace VkMusic2
{
    public class LyricPage : ContentPage
    {
        private Label Text;
        private ActivityIndicator indicator;
        private bool Searching = false;
        private SearchBar SearchLyric;
        private string LastID;
        public LyricPage()
        {
            this.Title = "Текст";
            indicator = new ActivityIndicator { Color = Color.Blue, IsRunning = true, IsVisible = false };
            SearchLyric = new SearchBar();
            SearchLyric.Placeholder = "Исполнитель - Название песни";
            if (Device.RuntimePlatform == Device.Android) SearchLyric.HeightRequest = 40.0;
            SearchLyric.SearchButtonPressed += 
                (i, e) => 
                    {
                        Match res = Regex.Match(SearchLyric.Text, @"(.+)-(.+)");
                        if (!res.Success) return;
                        Search(new Algh.Api.HttpUser.Audio { Author = res.Groups[1].Value, Name = res.Groups[2].Value, ID = LastID }, false);
                    };
            Text = new Label();
            Content = new ScrollView { Content = new StackLayout { Children = { SearchLyric, indicator, Text } } };
        }


        public async void Search(IAudio tr, bool setText)
        {
            if (Searching) return;
            Text.Text = "";
            LastID = tr.ID;
            if(setText) SearchLyric.Text = tr.Author + " - " + tr.Name;
            indicator.IsVisible = true;
            Searching = true;
            string res = await App.Lyrics.GetText(tr);
            res = res.Replace("\\n", "\n");
            Text.Text = res;
            Searching = false;
            indicator.IsVisible = false;
        }

    }
}
