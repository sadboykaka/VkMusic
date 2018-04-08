using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using Android.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VkMusic2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Music : ContentPage
	{

        

        SearchBar SearchMusic;

        string CurSearch;

        bool load = false;

        ActivityIndicator indicator = new ActivityIndicator { Color = Color.Blue, IsRunning = true, IsVisible = false };

        ObservableCollection<Algh.interfaces.IAudio> Tracks;
        //--------------------------------------------------------
        ListView listView;

        //----------------------------------------------------------

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var currentItem = e.Item as Algh.interfaces.IAudio;
            var lastItem = Tracks[Tracks.Count - 1];
            if (!load && currentItem == lastItem)
            {
                load = true;
                LoadTracks(CurSearch);
            }
            
        }

        void NewSearch(string name)
        {
            if (load) return;
            load = true;
            Tracks.Clear();
            CurSearch = name;
            LoadTracks(CurSearch);
        }

        void Entry_Completed(object sender, EventArgs e)
        {
            NewSearch(SearchMusic.Text);
        }

        public void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            var currentItem = e.Item as Algh.interfaces.IAudio;
            Collection.Player.Play(Collection.Player.TrackList[0].IndexOf(currentItem), 0);
        }

        private async void LoadTracks(string name)
        {
            indicator.IsVisible = true;
            IEnumerable<Algh.interfaces.IAudio> res;
            try
            {
                res = await Login.UserLogin.Data.Music.GetMusicFromSearch(name, Tracks.Count);
            }
            catch (Algh.exceptions.LoginException)
            {
                await DisplayAlert("Ошибка", "Необходимо перезайти в аккаунт", "OK");
                TabbedPage mp = (TabbedPage)Application.Current.MainPage;
                mp.CurrentPage = mp.Children[2];
                return;
            }
            catch
            {

                await DisplayAlert("Ошибка", "Ошибка загрузки!", "OK");
                load = false;
                indicator.IsVisible = false;
                return;
            }
            foreach (Algh.interfaces.IAudio a in res)
            {
                Tracks.Add(a);
            }
            load = false;
            indicator.IsVisible = false;
        }

        void PlayEvent(object sender, bool e)
        {
            if (!e) return;
            var pl = (Algh.interfaces.IPlayer)sender;
            var curplay = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
            if (listView.SelectedItem == curplay) return;
            if (pl.TrackList.Curtl != 0)
            {
                if (listView.SelectedItem != null) listView.SelectedItem = null;
                return;
            }
            listView.SelectedItem = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
        }

        public Music ()
		{
            Tracks = Collection.Player.TrackList[0];
            Collection.Player.PlayEvent += PlayEvent;

            SearchMusic = new SearchBar();
  
            if (Device.RuntimePlatform == Device.Android)
            {
                SearchMusic.HeightRequest = 40.0;
            }

            SearchMusic.SearchButtonPressed += Entry_Completed;

            

            listView = new Views.AudioDownloadList(Collection.ClickLoad, Tracks);

            listView.ItemAppearing += OnItemAppearing;
            listView.ItemTapped += OnItemTapped;
            listView.Header = SearchMusic;
            listView.Footer = indicator;

            if (Collection.Player.TrackList.Curtl == 0) listView.SelectedItem = Collection.Player.TrackList[0][Collection.Player.TrackList.Curid]; // check

            this.Content = new StackLayout { Children = { listView, new Views.adsView("ca-app-pub-5544910402146685/5107447214") } };
        }
	}
}