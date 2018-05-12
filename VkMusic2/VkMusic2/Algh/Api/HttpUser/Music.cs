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
using System.Net.Http.Headers;

namespace Algh.Api.HttpUser
{
    public class Music : IMusic
    {
        private Client Client;

        private string AddHash = null;
        private string DelHash = null;

        public Music(Client client)
        {
            Client = client;

        }

        private bool ParseAudioContent(string line, List<IAudio> res)
        {
            MatchCollection r = Regex.Matches(line, @"playPause\(event\, \'([0-9]+?_[0-9]+?)_(.+?)\', [\s\S]+?" + "<div class=\"ai_play\" style=\"(.*?)\">[\\s\\S]+?<span class=\"ai_title\">(.+?)</span>[\\s\\S]+?<span class=\"ai_artist\">(.+?)</span>[\\s\\S]+?<input type=\"hidden\" value=\"(.+?)\">");
            string buf;
            foreach (Match a in r)
            {
                buf = Decode.r(a.Groups[6].Value, Client.Id);
                if (buf == "") continue;
                string cover;
                if (a.Groups[3].Value == "") cover = "";
                else
                {
                    cover = Regex.Match(a.Groups[3].Value, @"url\((.+?)\)").Groups[1].Value;
                }

                res.Add(new Audio(buf, a.Groups[4].Value.Replace("<em class=\"found\">", ""), a.Groups[5].Value, a.Groups[1].Value, cover, a.Groups[2].Value));
            }
            if (AddHash == null || DelHash == null) GetHashes();

            return r.Count != 0;
        }

        private async void GetHashes()
        {
            string input = await Client.GetMobileVersion();
            Match res = Regex.Match(input, @"add_hash"":""(.+?)""\,""del_hash"":""(.+?)""");
            if (!res.Success) return;
            AddHash = res.Groups[1].Value;
            DelHash = res.Groups[2].Value;
        }

        public async Task<IEnumerable<IAudio>> GetMyMusic(int offset)
        {
            List<IAudio> res = new List<IAudio>();
            if (!Client.IsConnected) return res;
            string audiourl = Client.UserMusicPage;

            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("_ajax", "1"));
            sendp.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
            FormUrlEncodedContent sendpContent = new FormUrlEncodedContent(sendp);
            HttpResponseMessage postRes = await Client.loginClient.PostAsync(audiourl, sendpContent);

            if (!Client.CheckCookies()) throw new Algh.exceptions.LoginException();

            string output = await postRes.Content.ReadAsStringAsync();
            ParseAudioContent(output, res);
            return res;
        }

        public async void DeleteTrack(IAudio track)
        {
            if (!Client.IsConnected) throw new Exception("Client is not logined");
            if (DelHash == null) throw new Exception("DelHash is null");
            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("_ajax", "1"));
            sendp.Add(new KeyValuePair<string, string>("act", "delete"));
            sendp.Add(new KeyValuePair<string, string>("audio", track.ID + "_" + track.From));
            sendp.Add(new KeyValuePair<string, string>("hash", DelHash));
            FormUrlEncodedContent sendpContent = new FormUrlEncodedContent(sendp);
            HttpResponseMessage postRes = await Client.loginClient.PostAsync(Client.UserMusicPage, sendpContent);

            if (!Client.CheckCookies()) throw new Algh.exceptions.LoginException();
            postRes.EnsureSuccessStatusCode();
        }

        public async void AddTrack(IAudio track)
        {
            if (!Client.IsConnected) throw new Exception("Client is not logined");
            if (AddHash == null) throw new Exception("AddHash is null");
            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("_ajax", "1"));
            sendp.Add(new KeyValuePair<string, string>("act", "add"));
            sendp.Add(new KeyValuePair<string, string>("audio", track.ID + "_" + track.From));
            sendp.Add(new KeyValuePair<string, string>("hash", AddHash));
            FormUrlEncodedContent sendpContent = new FormUrlEncodedContent(sendp);
            HttpResponseMessage postRes = await Client.loginClient.PostAsync(Client.UserMusicPage, sendpContent);

            if (!Client.CheckCookies()) throw new Algh.exceptions.LoginException();
            postRes.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<IAudio>> GetMusicFromSearch(string name, int offset)
        {
            List<IAudio> res = new List<IAudio>();

            if (!Client.IsConnected) return res;

            string audiourl = Client.SearchPage + name;
            if (offset != 0) audiourl += "&offset=" + offset;
            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("_ajax", "1"));
            FormUrlEncodedContent sendpContent = new FormUrlEncodedContent(sendp);
            HttpResponseMessage postRes = await Client.loginClient.PostAsync(audiourl, sendpContent);

            if (!Client.CheckCookies()) throw new Algh.exceptions.LoginException();

            string output = await postRes.Content.ReadAsStringAsync();
            ParseAudioContent(output, res);
            return res;
        }

        public async Task<Stream> LoadTrack(IAudio tr)
        {
            var res = await Client.loginClient.GetAsync(tr.Url);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> LoadCover(IAudio tr)
        {
            var res = await Client.loginClient.GetAsync(tr.Cover);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStreamAsync();
        }

    }
}
