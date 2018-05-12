using Algh.interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VkMusic2
{
    public partial class Login : ContentPage
    {


        private Entry UserName = new Entry { Keyboard = Keyboard.Email };
        private Entry UserPassword = new Entry { IsPassword = true };

        private Button LoginButton = new Button();
        private Button LogoutButton = new Button();
        private Label textlogin = new Label { FontSize = 18, Text = "Попытка войти в аккаунт." };

        private Views.adsView ads = new Views.adsView(App.ads[2]);

        ActivityIndicator indicator = new ActivityIndicator { Color = Color.Blue, IsRunning = true, IsVisible = false };
        ActivityIndicator indicatorlogin = new ActivityIndicator { Color = Color.Blue, IsRunning = true, IsVisible = false };

        ObservableCollection<Algh.interfaces.IAudio> Tracks;

        bool load = false;

        public async void OnButtonClick(object sender, EventArgs e)
        {
            indicatorlogin.IsVisible = true;
            bool logined;
            try
            {
                logined = await App.UserLogin.Login(UserName.Text, UserPassword.Text);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "OK");
                indicatorlogin.IsVisible = false;
                return;
            }

            if (logined)
            {
                Render(2);
            }
            else
            {
                await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
            }
            indicatorlogin.IsVisible = false;
        }

        public void OnLogOutButtonClick(object sender, EventArgs e)
        {
            App.UserLogin.Logout();
            Render(1);
        }

        private async void LoadTracks()
        {
            indicator.IsVisible = true;
            IEnumerable<Algh.interfaces.IAudio> res;
            try
            {
                res = await App.UserLogin.Data.Music.GetMyMusic(Tracks.Count);
            }
            catch (Algh.exceptions.LoginException)
            {
                await DisplayAlert("Ошибка", "Необходимо перезайти в аккаунт", "OK");
                load = false;
                indicator.IsVisible = false;
                return;
            }
            catch
            {
                await DisplayAlert("Ошибка", "Ошибка загрузки!", "OK");
                load = false; 
                indicator.IsVisible = false;
                return;
            }

            foreach (Algh.Api.HttpUser.Audio a in res)
            {
                Tracks.Add(a);
            }
            load = false;
            indicator.IsVisible = false;
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var currentItem = e.Item as Algh.interfaces.IAudio;
            var lastItem = Tracks[Tracks.Count - 1];
            if (!load && currentItem == lastItem)
            {
                load = true;
                LoadTracks();
            }

        }

        ListView listView;

        public void OnItemTapped(object sender, ItemTappedEventArgs e)
        {

            var currentItem = e.Item as Algh.interfaces.IAudio;
            App.Player.Play(App.Player.TrackList[2].IndexOf(currentItem), 2);
        }

        void PlayEvent(object sender, bool e)
        {
            if (!e) return;
            var pl = (Algh.interfaces.IPlayer)sender;
            var curplay = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
            if (listView.SelectedItem == curplay) return;
            if (pl.TrackList.Curtl != 2)
            {
                if (listView.SelectedItem != null) listView.SelectedItem = null;
                return;
            }
            listView.SelectedItem = pl.TrackList[pl.TrackList.Curtl][pl.TrackList.Curid];
        }

        void Render(short type)
        {
            if (type == 0)
            {
                indicatorlogin.IsVisible = true;
                this.Content = new StackLayout { HorizontalOptions = LayoutOptions.Center, Children = { textlogin, indicatorlogin } };
            }
            else if (type == 1)
            {
                indicatorlogin.IsVisible = false;
                this.Content = new StackLayout { Children = { UserName, UserPassword, LoginButton, indicatorlogin } };
            }
            else if (type == 2)
            {
                Tracks.Clear();
                this.Content = new StackLayout { Children = { listView, LogoutButton, ads } };
                LoadTracks();
            }
            else if (type == 3)
            {
                this.Content = new StackLayout { Children = { listView, LogoutButton, ads } };
            }
        }

        public void TryLogin()
        {
            if (Tracks.Count != 0)
            {
                Render(3);
                var a = App.Player.TrackList;
                if (App.Player.TrackList.Curtl == 2) listView.SelectedItem = App.Player.TrackList[2][App.Player.TrackList.Curid]; // check
                return;
            }
                 
            Render(0);
            if (App.UserLogin.Login())
            {
                Render(2);
            }
            else Render(1);
           
        }

        public Login()
        {
            Tracks = App.Player.TrackList[2];
            App.Player.PlayEvent += PlayEvent;

            var refreshIcon = new Views.ClickableIcon(ImageSource.FromFile("ic_refresh_grey600_36dp.png"), (i, e) => { Tracks.Clear(); LoadTracks(); }) { HorizontalOptions = LayoutOptions.StartAndExpand } ;

            LoginButton.Clicked += OnButtonClick;
            LogoutButton.Clicked += OnLogOutButtonClick;
            LoginButton.Text = "Войти";
            LogoutButton.Text = "Выйти";
            UserName.Placeholder = "Почта/Телефон";
            UserPassword.Placeholder = "Пароль";

            listView = new Views.AudioDownloadList(
                (i, e) => { var item = ((Image)i).BindingContext as IAudio; App.trackManager.LoadTrack(item, App.Player.TrackList[1]); }, 
                Tracks, App.Player.TrackList[1],
                (i,e) => { var item = ((Image)i).BindingContext as IAudio; try { App.UserLogin.Data.Music.DeleteTrack(item); Tracks.Remove(item); } catch { DisplayAlert("Ошибка", "Комманда не может быть выполнена!", "OK"); } },
                false
                );
            listView.Header = refreshIcon;
            listView.ItemAppearing += OnItemAppearing;
            listView.ItemTapped += OnItemTapped;
            listView.Footer = indicator;
            TryLogin();
        }
    }
}