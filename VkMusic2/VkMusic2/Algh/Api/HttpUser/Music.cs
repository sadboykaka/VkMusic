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
    public class Music : IMusic
    {
        private Client Client;

        public Music(Client client)
        {
            Client = client;
        }

        private bool ParseAudioContent(string line, List<IAudio> res)
        {
            MatchCollection r = Regex.Matches(line, "<span class=\"ai_title\">(.+?)</span>\n.+?\n<span class=\"ai_artist\">(.+?)</span>\n</div>\n<input type=\"hidden\" value=\"(.+?)\">");
            string buf;
            foreach (Match a in r)
            {
                buf = Decode.r(a.Groups[3].Value.ToString());
                if (buf == "") continue;
                res.Add(new Audio(buf, a.Groups[1].Value.ToString().Replace("<em class=\"found\">", ""), a.Groups[2].Value.ToString()));
            }
            return r.Count != 0;
        }

        public async Task<IEnumerable<IAudio>> GetMyMusic(int offset)
        {
            List<IAudio> res = new List<IAudio>();
            if (!Client.IsConnected) return res;
            string audiourl = Client.UserMusicPage;
            List<KeyValuePair<string, string>> sendp = new List<KeyValuePair<string, string>>();
            sendp.Add(new KeyValuePair<string, string>("_ajax", "1"));
            sendp.Add(new KeyValuePair<string, string>("offset", offset.ToString()));
            FormUrlEncodedContent sendpContent;
            sendpContent = new FormUrlEncodedContent(sendp);
            HttpResponseMessage postRes = await Client.loginClient.PostAsync(audiourl, sendpContent);

            if (!Client.CheckCookies()) throw new Algh.exceptions.LoginException();
            


            //if (UserMusicPage != postRes.RequestMessage.RequestUri.ToString()) return res;
            ParseAudioContent(await postRes.Content.ReadAsStringAsync(), res);

            return res;
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


            ParseAudioContent(await postRes.Content.ReadAsStringAsync(), res);

            return res;
        }

        public async Task<Stream> LoadTrack(IAudio tr)
        {
            var res = await Client.loginClient.GetAsync(tr.Url);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStreamAsync();
        }


    }
}
