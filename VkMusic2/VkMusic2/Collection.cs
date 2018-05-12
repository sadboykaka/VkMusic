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
    public partial class Collection : ContentPage
    {

        ObservableCollection<IAudio> Tracks;

        Downloads downloadspage;

        Views.AudioListViewWithCheckBoxes listViewTracks;

        Algh.SearchHelper searchHelper;

        Views.ClickableHashIcon loadb;
        Views.ClickableHashIcon deleteb;

        private bool menuOpened = false;

        private void LoadEvent(object o, IAudio a)
        {
            Tracks.Add(a);
        }
        public void OnItemTapped(object sender, ItemTappedEventArgs e)
        {

            var currentItem = e.Item as IAudio;
            App.Player.Play(App.Player.TrackList[1].IndexOf(currentItem), 1);
        }

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
            IEnumerable<IAudio> delt = listViewTracks.GetSelectedItems();

            if (delt == null) return;

            App.trackManager.DeleteTracks(delt);
            ObservableCollection<IAudio> viewT = (ObservableCollection<IAudio>)listViewTracks.ItemsSource;
            bool viewTracks = Tracks != viewT;
            var tl = App.Player.TrackList[App.Player.TrackList.Curtl];
            foreach (var track in delt)
            {
                if (tl != null && track == tl[App.Player.TrackList.Curid]) App.Player.Stop();
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
        ~Collection()
        {
        }

        public Collection()
        {
            Tracks = App.Player.TrackList[1];

            App.Player.PlayEvent += PlayEvent;

            loadb = new Views.ClickableHashIcon("ic_download_grey600_36dp.png", MenuButton1Tap);
            deleteb = new Views.ClickableHashIcon("ic_delete_grey600_36dp.png", MenuButton2Tap);

            var menu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };



            menu.Children.Add(loadb);
            menu.Children.Add(deleteb);

            Views.AudioListViewWithOutSelect DownloadList = new Views.AudioListViewWithOutSelect(App.trackManager.Downloads);
            downloadspage = new Downloads(DownloadList);

            listViewTracks = new Views.AudioListViewWithCheckBoxes(Tracks);



            listViewTracks.ItemTapped += OnItemTapped;

            var ads = new Views.adsView(App.ads[1]);
            //---------------------------------------Search-------------------------------------
            var SearchEntry = new SearchBar();
            if (Device.RuntimePlatform == Device.Android) SearchEntry.HeightRequest = 40.0;
     
            listViewTracks.Header = SearchEntry;


            searchHelper = new Algh.SearchHelper(Tracks);
            searchHelper.SearchEvent += (i, e) => 
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var buf = listViewTracks.SelectedItem;
                    listViewTracks.ItemsSource = null;
                    listViewTracks.ItemsSource = e;
                    listViewTracks.SelectedItem = null;
                    listViewTracks.SelectedItem = buf;

                });
                
            };

            

            SearchEntry.TextChanged += (i, e) => { searchHelper.Search(e.NewTextValue); };
            //-----------------------------------------------------------------------------------
            if (App.Player.TrackList.Curtl != -1)
            {
                if (App.Player.TrackList.Curtl == 1) listViewTracks.SelectedItem = App.Player.TrackList[1][App.Player.TrackList.Curid]; // check
            }
            else foreach (IAudio track in App.trackManager.GetTracks()) Tracks.Add(track);
            //------------------------------------------------------------------------------------

            this.Content = new StackLayout { Children = { listViewTracks, new Views.AudioPlayerView(App.Player, menu), ads } };



        }
    }
}