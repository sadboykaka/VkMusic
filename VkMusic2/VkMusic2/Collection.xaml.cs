using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;
using Android.Media;
using Algh.Api.HttpUser;
using Algh.interfaces;

namespace VkMusic2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    

    public partial class Collection : ContentPage
	{
        static ITrackManager trackManager = new FileManager<Audio>(Cookies.appdir, Login.UserLogin);

        public static IPlayer Player = new Algh.Api.AndroidAudioServicePlayer(3);


        ObservableCollection<Algh.interfaces.IAudio> Tracks;
        ObservableCollection<Algh.interfaces.IAudio> Downloads;

        public static Collection curCollection;

        Downloads downloadspage;

        Views.AudioListViewWithCheckBoxes listViewTracks;

        Algh.SearchHelper searchHelper;

        

        public static void ClickLoad(object sender, EventArgs e)
        {
            
            var item = ((Xamarin.Forms.Image)sender).BindingContext as Algh.interfaces.IAudio;
            curCollection.StartLoad(item);
        }

        public async void StartLoad(IAudio item)
        { 
            IAudio track = null;
            try
            {
                Downloads.Add(item);
                track = await trackManager.LoadTrack(item);

            }
            catch
            {
                await DisplayAlert("Ошибка", "Ошибка загрузки!", "OK");
            }
            finally
            {
                if (track != null) Tracks.Add(track);
                Downloads.Remove(item);
            }

        }

        public void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
           
            var currentItem = e.Item as Algh.interfaces.IAudio;
            Player.Play(Player.TrackList[1].IndexOf(currentItem), 1);
        }

        Views.ClickableHashIcon loadb;
        Views.ClickableHashIcon deleteb;

        private bool menuOpened = false;

        private void RenderMenu(bool f)
        {
            if (f)
            {
                loadb.Source = ImageSource.FromFile("ic_arrow_left_grey600_36dp.png");
                deleteb.Source = ImageSource.FromFile("ic_check_grey600_36dp.png");
            }
            else
            {
                loadb.Source = ImageSource.FromFile("ic_download_grey600_36dp.png");
                deleteb.Source = ImageSource.FromFile("ic_delete_grey600_36dp.png");
            }
            menuOpened = f;
        }

        private void MenuButton1Tap(object sender, EventArgs e)
        {
            if (menuOpened)
            {
                listViewTracks.setCheckBoxes(false);
                RenderMenu(false);
            }
            else Navigation.PushAsync(downloadspage); 

        }

        private void MenuButton2Tap(object sender, EventArgs e)
        {
            if (menuOpened)
            {
                DeleteAllSelectedItems();
                RenderMenu(false);
            }
            else
            {
                listViewTracks.setCheckBoxes(true);
                RenderMenu(true);
            }
            

        }

        private void DeleteAllSelectedItems()
        {
            IEnumerable<Algh.interfaces.IAudio> delt = listViewTracks.GetSelectedItems();

            if (delt == null) return;

            Player.Stop();

            trackManager.DeleteTracks(delt);
            ObservableCollection<IAudio> viewT = (ObservableCollection< IAudio >)listViewTracks.ItemsSource;
            bool viewTracks = Tracks != viewT;
            foreach (var track in delt)
            {
                if (viewTracks) viewT.Remove(track);
                Tracks.Remove(track);
            }

            listViewTracks.setCheckBoxes(false);
        }

        void PlayEvent(object sender, bool e)
        {
            if (!e) return;
            var pl = (Algh.interfaces.IPlayer)sender;
            var curplay = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
            if (listViewTracks.SelectedItem == curplay) return;
            if (pl.TrackList.Curtl != 1)
            {
                if (listViewTracks.SelectedItem != null) listViewTracks.SelectedItem = null;
                return;
            }
            listViewTracks.SelectedItem = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
        }



        public Collection ()
	    {
            Tracks = Player.TrackList[1];
           
            curCollection = this;
            //ic_delete_white_18dp.png

            Player.PlayEvent += PlayEvent;

            loadb = new Views.ClickableHashIcon("ic_download_grey600_36dp.png", MenuButton1Tap);
            deleteb = new Views.ClickableHashIcon("ic_delete_grey600_36dp.png", MenuButton2Tap);

            var menu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };



            menu.Children.Add(loadb);
            menu.Children.Add(deleteb);

            Downloads = new ObservableCollection<IAudio>();
            
            Views.AudioListViewWithOutSelect DownloadList = new Views.AudioListViewWithOutSelect(Downloads);
            downloadspage = new Downloads(DownloadList);
            listViewTracks = new Views.AudioListViewWithCheckBoxes(Tracks);

            

            listViewTracks.ItemTapped += OnItemTapped;

            var ads = new Views.adsView("ca-app-pub-5544910402146685/8051506455");
            //---------------------------------------Search-------------------------------------
            var SearchEntry = new SearchBar();

            searchHelper = new Algh.SearchHelper(Tracks);
            searchHelper.SearchEvent += (i, e) => { listViewTracks.ItemsSource = e; };
            SearchEntry.TextChanged += (i, e) => { listViewTracks.ItemsSource = null; searchHelper.Search(e.NewTextValue); };
            if (Device.RuntimePlatform == Device.Android)
            {
                SearchEntry.HeightRequest = 40.0;
            }

            listViewTracks.Header = SearchEntry;
            //-----------------------------------------------------------------------------------
            if(Player.TrackList.Curtl != -1)
            {
                if (Player.TrackList.Curtl == 1) listViewTracks.SelectedItem = Player.TrackList[1][Player.TrackList.Curid]; // check
                

            }
            else
            {
                foreach (IAudio track in trackManager.GetTracks()) Tracks.Add(track);
                
            }


            

            this.Content = new StackLayout { Children = {listViewTracks, new Views.AudioPlayerView(Player, menu), ads } };

            

        }
	}
}