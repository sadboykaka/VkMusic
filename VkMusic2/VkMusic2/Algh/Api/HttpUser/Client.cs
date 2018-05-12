using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using Algh.interfaces;

namespace Algh.Api.HttpUser
{
    public class Client : IUser
    {
        HttpClientHandler loginHandler;

        public HttpClient loginClient { get; }

        public const string startPage = "https://vk.com";
        public const string loginPage = "https://login.vk.com/?act=login";
        public const string musicPage = "https://m.vk.com/audios";
        public const string musicEnginePage = "https://vk.com/al_audio.php";
        public const string SearchPage = "https://m.vk.com/audio?act=search&q=";
        public const string UserMusicPage = "https://m.vk.com/audio";

        public int Id { get; private set; }

        public IData Data { get; private set; }

        public bool IsConnected { get; private set; }

        public Client()
        {
            IsConnected = false;
            Data = new Data(this);
            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
            loginHandler = new HttpClientHandler();
            loginHandler.AllowAutoRedirect = true;
            loginHandler.UseCookies = true;
            loginHandler.CookieContainer = new CookieContainer();
            loginHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            loginHandler.UseProxy = false;
            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------
            loginClient = new HttpClient(loginHandler) { Timeout = TimeSpan.FromSeconds(150) };
            loginClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            loginClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
            loginClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
            loginClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            loginClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36");
            //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        }

        public async Task<string> GetMobileVersion()
        {
            loginHandler.CookieContainer.Add(new Cookie("remixmdevice", "1920/1080/1/!!-!!!!") { Domain = "m.vk.com" });
            loginHandler.CookieContainer.Add(new Cookie("remixdt", "0") { Domain = "m.vk.com" });
            loginHandler.CookieContainer.Add(new Cookie("remixseenads", "2") { Domain = "m.vk.com" });
            loginHandler.CookieContainer.Add(new Cookie("remixflash", "27.0.0") { Domain = "m.vk.com" });
            loginHandler.CookieContainer.Add(new Cookie("remixscreen_depth", "24") { Domain = "m.vk.com" });
            loginHandler.CookieContainer.Add(new Cookie("remixab", "1") { Domain = "m.vk.com" });
            var resI = await loginClient.GetAsync("https://m.vk.com/audio");
            return await resI.Content.ReadAsStringAsync();
        }

        public bool CheckCookies()
        {
            var a = loginHandler.CookieContainer.GetCookies(new Uri(startPage));
            foreach(Cookie c in a)
            {
                if (c.Name == "remixsid") return true;
            }
            return false;
        }

        public async Task<string> GetHTML(string url)
        {
            byte[] bytes = await loginClient.GetByteArrayAsync(new Uri(url));
            Encoding encoding = Encoding.GetEncoding("Windows-1251");
            return encoding.GetString(bytes, 0, bytes.Length);
        }

        private async Task<FormUrlEncodedContent> GetLoginContent(string login, string pass)
        {
            string loginPageHTML = await GetHTML(startPage);

            string ip_h = Regex.Match(loginPageHTML, "input type=\"hidden\" name=\"ip_h\" value=\"(.+)\"").Groups[1].Value.ToString();
            string lg_h = Regex.Match(loginPageHTML, "input type=\"hidden\" name=\"lg_h\" value=\"(.+)\"").Groups[1].Value.ToString();

            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("act", "login"));
            sendp.Add(new KeyValuePair<string, string>("role", "al_frame"));
            sendp.Add(new KeyValuePair<string, string>("expire", ""));
            sendp.Add(new KeyValuePair<string, string>("recaptcha", ""));
            sendp.Add(new KeyValuePair<string, string>("captcha_sid", ""));
            sendp.Add(new KeyValuePair<string, string>("captcha_key", ""));
            sendp.Add(new KeyValuePair<string, string>("_origin", startPage));
            sendp.Add(new KeyValuePair<string, string>("ip_h", ip_h));
            sendp.Add(new KeyValuePair<string, string>("lg_h", lg_h));
            sendp.Add(new KeyValuePair<string, string>("email", login));
            sendp.Add(new KeyValuePair<string, string>("pass", pass));

          
            return new FormUrlEncodedContent(sendp);

        }

        private bool CheckLogin(HttpResponseMessage res)
        {
            return Regex.IsMatch(res.RequestMessage.RequestUri.ToString(), "hash");
        }

        public bool Login()
        {
            if (IsConnected) return true;
            CookieContainer res = Cookies.ReadCookiesFromDisk(Cookies.BaseName);
            if (res == null) return false;
            loginHandler.CookieContainer = res;
            IsConnected = true;
            Id = GetId();
            if (CheckCookies() && Id != -1) return true;
            else
            {
                Cookies.DeleteCookies(Cookies.BaseName);
                return false;
            }
         
        }

        private int GetId()
        {
            var ch = loginHandler.CookieContainer.GetCookies(new Uri("https://login.vk.com"));
            try
            {
                var e = ch["l"].Value;
                return Convert.ToInt32(ch["l"].Value);
            }
            catch {
                return -1;
            }
        }

        public async Task<bool> Login(string login, string pass)
        {
            if (IsConnected) return false;
            var sendpContent = await GetLoginContent(login, pass);
            if (!CheckLogin(await loginClient.PostAsync(loginPage, sendpContent))) return false;
            //-------------------------------------------------------------------
            Id = GetId();
            if (Id == -1) return false;
            Cookies.WriteCookiesToDisk(Cookies.BaseName, loginHandler.CookieContainer);
            IsConnected = true;
            return true;
        }

        public async void Logout()
        {
            if (!IsConnected) return;
            IsConnected = false;
            Cookies.DeleteCookies(Cookies.BaseName);
            await loginClient.GetAsync(await GetExitLink());
            
        }

        private async Task<string> GetExitLink()
        {
            string testLogin = await GetHTML(startPage);
            Regex testRegex = new Regex(@"https://login.vk.com/\?act=logout&hash=.+&reason=tn&_origin=https://vk.com"); // поиск ссылки на выход
            Match testMatch = testRegex.Match(testLogin);
            if (testMatch.Success) return testMatch.Value.ToString();
            else return null;
        }

    }
}
