using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Algh.interfaces;
using Algh.Api;

using Algh.Api.HttpUser;

namespace VkMusic2
{
    public partial class App : Application
    {
        public static IUser UserLogin = new Client();
        public static ITrackManager trackManager = new FileManager<Audio>(Cookies.appdir, UserLogin);
        public static IPlayer Player = new AndroidPlayer(3);
        public static ILyrics Lyrics = new HttpLyricApi((Client)UserLogin, Cookies.appdir);
        public LyricPage lyricPage { get; private set;}
        public static readonly string[] ads = { "ca-app-pub-5544910402146685/5107447214", "ca-app-pub-5544910402146685/4915875521", "ca-app-pub-5544910402146685/8051506455" };

        public App()
        {
            InitializeComponent();
            lyricPage = new LyricPage();
            MainPage = new NavigationPage(new VkMusic2.MainPage());

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}