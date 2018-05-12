using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Algh.interfaces;
using System.Threading.Tasks;

namespace Algh.Api.HttpUser
{
    class HttpLyricApi : ILyrics
    {
        Client User;

        string Path;

        public HttpLyricApi(Client user, string path)
        {
            User = user;
            Path = path + "/Lyrics/";
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
        }

        string GetTextFromFile(IAudio track)
        {
            string textpath = Path + track.ID + ".txt";
            if (!File.Exists(textpath)) return "";
            else return File.ReadAllText(textpath);
        }

        void WriteTextToFile(IAudio track, string text)
        {
            string textpath = Path + track.ID + ".txt";
            File.WriteAllText(textpath, text);
        }

        public async Task<string> GetText(IAudio track)
        {
            if (User == null || track == null) return "";
            string fres = GetTextFromFile(track);
            if (fres != "") return fres;
            string p = "http://lyric-api.herokuapp.com/api/find/" + track.Author.Trim() + '/' + track.Name.Trim();
            var res = await User.GetHTML(p);
            var a = Regex.Match(res, @"""lyric"":""([\s\S]+?)"",");
            if (!a.Success) return "";
            WriteTextToFile(track, a.Groups[1].Value);
            return a.Groups[1].Value;
        }
    }
}
