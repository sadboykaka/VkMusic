using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Algh.interfaces;
using System.Threading.Tasks;
using System.Threading;

namespace Algh.Api.HttpUser
{
    class FileManager<T> : ITrackManager where T : IAudio, new()
    {
        public string dirpath { get; private set; }
        private IUser UserLogin;
        public FileManager(string p, IUser user)
        {
            dirpath = p;
            UserLogin = user;
        }

        private IAudio GetIAudio(string path)
        {
            Match buf = Regex.Match(Path.GetFileName(path), @"(.+) - (.+)\.");
            if (!buf.Success) return null;
            var tr = new T();
            tr.Url = path;
            tr.Author = buf.Groups[1].Value;
            tr.Name = buf.Groups[2].Value;
            return tr;
        }

        public IEnumerable<IAudio> GetTracks()
        {
            var dir = dirpath + "/Audio";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var tracks = Directory.GetFiles(dir);

            var res = new List<IAudio>();
            IAudio track;
            foreach (string a in tracks)
            {
                track = GetIAudio(a);
                if (track == null) continue;
                res.Add(track);
            }
            return res;
        }

        public void DeleteTracks(IEnumerable<IAudio> trs)
        {
            var dir = dirpath + "/IAudio/";
            foreach (IAudio tr in trs)
            {
                if (!File.Exists(tr.Url)) continue;
                File.Delete(tr.Url);
            }
        }

        public async Task<IAudio> LoadTrack(IAudio tr)
        {
            var path = dirpath + "/Audio";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var fileNameMask = path + '/' + tr.Author + " - " + tr.Name;
            var filepath = fileNameMask + ".mp3";

            Stream stream;
            try
            {
                stream = await UserLogin.Data.Music.LoadTrack(tr);
            }
            catch
            {
                return null;
            }
            int i = 1;
            while (true)
            {
                if (File.Exists(filepath)) filepath = fileNameMask + "(" + i + ").mp3";
                else break;
            }


            try
            {
                using (var fileStream = File.Create(filepath))
                {
                    stream.CopyTo(fileStream);
                }
            }
            catch
            {
                return null;
            }
            return GetIAudio(filepath);
        }

    }
}
